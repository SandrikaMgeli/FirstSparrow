CREATE TABLE IF NOT EXISTS restaurants 
(
    id SERIAL PRIMARY KEY,
    name VARCHAR(1000) NOT NULL,
    password VARCHAR(1000) NOT NULL,
    restaurant_flags INTEGER NOT NULL,
    create_timestamp TIMESTAMPTZ,
    update_timestamp TIMESTAMPTZ,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);