CREATE TABLE IF NOT EXISTS merkle_nodes
(
    id SERIAL PRIMARY KEY,
    commitment VARCHAR(500) NOT NULL,
    index BIGINT NOT NULL,
    layer INTEGER NOT NULL,
    deposit_timestamp TIMESTAMPTZ DEFAULT NULL,
    create_timestamp TIMESTAMPTZ NOT NULL,
    update_timestamp TIMESTAMPTZ DEFAULT NULL,
    is_deleted BOOLEAN DEFAULT FALSE NOT NULL
);

CREATE INDEX IF NOT EXISTS index_layer_idx ON merkle_nodes (index, layer);

CREATE INDEX IF NOT EXISTS commitment_idx on merkle_nodes (commitment);