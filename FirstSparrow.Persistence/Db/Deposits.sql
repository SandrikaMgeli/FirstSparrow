CREATE TABLE IF NOT EXISTS deposits
(
    id SERIAL PRIMARY KEY,
    commitment VARCHAR(500) NOT NULL,
    leaf_index BIGINT NOT NULL,
    deposit_timestamp TIMESTAMPTZ NOT NULL,
    create_timestamp TIMESTAMPTZ NOT NULL,
    update_timestamp TIMESTAMPTZ DEFAULT NULL,
    is_deleted BOOLEAN DEFAULT FALSE NOT NULL
);