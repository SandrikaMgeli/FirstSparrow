using FirstSparrow.Application.Domain.Entities;

namespace FirstSparrow.Persistence.Constants;

public static class RestaurantRepositoryQueries
{
    public const string InsertQuery = $@"INSERT INTO restaurants
                                        (
                                            name,
                                            owner_phone_number,
                                            owner_name,
                                            owner_last_name,
                                            owner_personal_number,
                                            restaurant_flags,
                                            create_timestamp,
                                            update_timestamp
                                        )
                                    VALUES
                                        (
                                            @{nameof(Restaurant.Name)},
                                            @{nameof(Restaurant.OwnerPhoneNumber)},
                                            @{nameof(Restaurant.OwnerName)},
                                            @{nameof(Restaurant.OwnerLastName)},
                                            @{nameof(Restaurant.OwnerPersonalNumber)},
                                            @{nameof(Restaurant.RestaurantFlags)},
                                            @{nameof(Restaurant.CreateTimestamp)},
                                            @{nameof(Restaurant.UpdateTimestamp)}
                                        )
                                    RETURNING id;";

    public const string UpdateQuery = @$"UPDATE restaurants
                                            SET
                                                name = @{nameof(Restaurant.Name)},
                                                owner_phone_number = @{nameof(Restaurant.OwnerPhoneNumber)},
                                                owner_name = @{nameof(Restaurant.OwnerName)},
                                                owner_last_name = @{nameof(Restaurant.OwnerLastName)},
                                                owner_personal_number = @{nameof(Restaurant.OwnerPersonalNumber)},
                                                is_onboarded = @{nameof(Restaurant.RestaurantFlags)},
                                                create_timestamp = @{nameof(Restaurant.CreateTimestamp)},
                                                update_timestamp = @{nameof(Restaurant.UpdateTimestamp)}
                                            WHERE id = @{nameof(Restaurant.Id)};";

    public const string DeleteQuery = @$"SELECT
                                            id as {nameof(Restaurant.Id)}
                                            name as {nameof(Restaurant.Name)}
                                            owner_phone_number as {nameof(Restaurant.OwnerPhoneNumber)}
                                            owner_name as {nameof(Restaurant.OwnerName)}
                                            owner_last_name as {nameof(Restaurant.OwnerLastName)}
                                            owner_personal_number as {nameof(Restaurant.OwnerPersonalNumber)}
                                            is_onboarded as {nameof(Restaurant.RestaurantFlags)}
                                            create_timestamp as {nameof(Restaurant.CreateTimestamp)}
                                            update_timestamp as {nameof(Restaurant.UpdateTimestamp)}
                                        FROM restaurants WHERE id = @{nameof(Restaurant.Id)};";

    public const string GetByIdQuery = "";
}