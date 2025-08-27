using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solutions.TodoList.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateMaterializedTodosAndTriggers_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Create denormalized table if missing (lowercase columns)
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS public.materialized_todos (
                    id uuid PRIMARY KEY,
                    title text NOT NULL,
                    description text,
                    done boolean NOT NULL DEFAULT false,
                    createdat timestamp with time zone NOT NULL,
                    updatedat timestamp with time zone,
                    completedat timestamp with time zone,
                    user_id uuid NOT NULL
                );
            ");

            // 2) Indexes for common queries
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_materialized_todos_user_id ON public.materialized_todos (user_id);
                CREATE INDEX IF NOT EXISTS idx_materialized_todos_createdat ON public.materialized_todos (createdat);
            ");

            // 3) Trigger function (idempotent via CREATE OR REPLACE)
            // Note: we use NEW.""ColumnName"" to read from the Todos row in case the Todos table/columns are quoted/camel-cased.
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.sync_materialized_todos()
                RETURNS trigger AS $$
                BEGIN
                    IF (TG_OP = 'INSERT' OR TG_OP = 'UPDATE') THEN
                        INSERT INTO public.materialized_todos (id, title, description, done, createdat, updatedat, completedat, user_id)
                        VALUES (
                            NEW.""Id"",
                            NEW.""Title"",
                            NEW.""Description"",
                            NEW.""Done"",
                            NEW.""CreatedAtUtc"",
                            NEW.""UpdatedAtUtc"",
                            NEW.""CompletedAtUtc"",
                            NEW.""UserId""
                        )
                        ON CONFLICT (id) DO UPDATE
                        SET
                            title = EXCLUDED.title,
                            description = EXCLUDED.description,
                            done = EXCLUDED.done,
                            createdat = EXCLUDED.createdat,
                            updatedat = EXCLUDED.updatedat,
                            completedat = EXCLUDED.completedat,
                            user_id = EXCLUDED.user_id;
                        RETURN NEW;
                    ELSIF (TG_OP = 'DELETE') THEN
                        DELETE FROM public.materialized_todos WHERE id = OLD.""Id"";
                        RETURN OLD;
                    END IF;
                    RETURN NULL;
                END;
                $$ LANGUAGE plpgsql;
            ");

            // 4) Create trigger (drop first if exists to be safe)
            // Note: use schema-qualified, quoted table name for Todos (matches your DB listing which shows 'Todos')
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS tr_sync_materialized_todos ON public.""Todos"";
                CREATE TRIGGER tr_sync_materialized_todos
                AFTER INSERT OR UPDATE OR DELETE ON public.""Todos""
                FOR EACH ROW EXECUTE FUNCTION public.sync_materialized_todos();
            ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS tr_sync_materialized_todos ON public.""Todos"";
            ");

            migrationBuilder.Sql(@"
                DROP FUNCTION IF EXISTS public.sync_materialized_todos();
            ");

            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS public.materialized_todos;
            ");
        }
    }
}
