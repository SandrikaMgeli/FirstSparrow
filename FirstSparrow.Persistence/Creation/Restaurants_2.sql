CREATE TABLE IF NOT EXISTS restaurants
(
    id SERIAL PRIMARY KEY,
    name VARCHAR(200),
    owner_phone_number VARCHAR(200),
    owner_name VARCHAR(200),
    owner_last_name VARCHAR(200),
    owner_personal_number VARCHAR(200),
    restaurant_flags INTEGER,
    create_timestamp TIMESTAMPTZ,
    update_timestamp TIMESTAMPTZ
);