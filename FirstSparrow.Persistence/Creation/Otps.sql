CREATE TABLE otps
(
    id SERIAL PRIMARY KEY,
    code int,
    destination varchar(200),
    is_used bool,
    expires_at TIMESTAMPTZ,
    create_timestamp TIMESTAMPTZ,
    update_timestamp TIMESTAMPTZ
);