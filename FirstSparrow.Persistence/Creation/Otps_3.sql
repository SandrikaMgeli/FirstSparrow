CREATE TABLE IF NOT EXISTS otps
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

-- Create indexes if not exist
CREATE INDEX IF NOT EXISTS otp_idx_sent_destination_usage_used
    ON otps (destination, usage)
    WHERE is_sent = TRUE AND is_used = FALSE;

CREATE INDEX IF NOT EXISTS otp_idx_sent
    ON otps (is_sent);