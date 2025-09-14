export default function AuthCard({
    title,
    children,
    footer,
}: {
    title?: string;
    children: React.ReactNode;
    footer?: React.ReactNode;
}) {
    return (
        <div className="auth-card">
            {title && <h3 className="auth-card__title">{title}</h3>}
            <div className="auth-card__body">{children}</div>
            {footer && <div className="auth-card__footer">{footer}</div>}
        </div>
    );
}