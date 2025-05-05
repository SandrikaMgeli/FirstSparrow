using FirstSparrow.Application.Domain.Entities;

namespace FirstSparrow.Persistence.Constants;

public static class RestaurantRepositoryQueries
{
    public const string InsertQuery = $@"INSERT INTO restaurants
                                        (
                                            name,
                                            password,
                                            restaurant_flags,
                                            create_timestamp,
                                            update_timestamp,
                                            is_deleted,
                                        )
                                    VALUES
                                        (
                                            @{nameof(Restaurant.Name)},
                                            @{nameof(Restaurant.Password)},
                                            @{nameof(Restaurant.RestaurantFlags)},
                                            @{nameof(Restaurant.CreateTimestamp)},
                                            @{nameof(Restaurant.UpdateTimestamp)},
                                            @{nameof(Restaurant.IsDeleted)}
                                        )
                                    RETURNING id;";

    public const string UpdateQuery = @$"UPDATE restaurants
                                            SET
                                                name = @{nameof(Restaurant.Name)},
                                                password = @{nameof(Restaurant.Password)},
                                                restaurant_flags = @{nameof(Restaurant.RestaurantFlags)},
                                                create_timestamp = @{nameof(Restaurant.CreateTimestamp)},
                                                update_timestamp = @{nameof(Restaurant.UpdateTimestamp)},
                                                is_deleted = @{nameof(Restaurant.IsDeleted)}
                                            WHERE id = @{nameof(Restaurant.Id)};";

    public const string DeleteQuery = @$"UPDATE restaurants
                                            SET
                                                is_deleted = TRUE
                                            WHERE id = @Id;";

    public const string GetByIdQuery = $@"SELECT
                                            id as {nameof(Restaurant.Id)},
                                            name as {nameof(Restaurant.Name)},
                                            password as {nameof(Restaurant.Password)},
                                            restaurant_flags as {nameof(Restaurant.RestaurantFlags)},
                                            create_timestamp as {nameof(Restaurant.CreateTimestamp)},
                                            update_timestamp as {nameof(Restaurant.UpdateTimestamp)}
                                        FROM restaurants WHERE id = @Id;";
}