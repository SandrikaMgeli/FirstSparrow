CREATE TABLE otps
(
    id SERIAL PRIMARY KEY,
    code int,
    destination varchar(200),
    is_used bool,
    usage int,
    is_sent bool DEFAULT FALSE,
    expires_at TIMESTAMPTZ,
    create_timestamp TIMESTAMPTZ,
    update_timestamp TIMESTAMPTZ
);