using FirstSparrow.Application.Domain.Entities;

namespace FirstSparrow.Persistence.Constants;

public static class OtpRepositoryQueries
{
    public const string InsertQuery = $@"INSERT INTO otps
                                        (
                                            code,
                                            is_used,
                                            usage,
                                            is_sent,
                                            destination,
                                            expires_at,
                                            create_timestamp,
                                            update_timestamp
                                        )
                                        VALUES
                                        (
                                            @{nameof(Otp.Code)},
                                            @{nameof(Otp.IsUsed)},
                                            @{nameof(Otp.Usage)},
                                            @{nameof(Otp.IsSent)},
                                            @{nameof(Otp.Destination)},
                                            @{nameof(Otp.ExpiresAt)},
                                            @{nameof(Otp.CreateTimestamp)},
                                            @{nameof(Otp.UpdateTimestamp)}
                                        ) 
                                        RETURNING id;";

    public const string UpdateQuery = @$"UPDATE otps
                                            SET
                                                code = @{nameof(Otp.Code)},
                                                destination = @{nameof(Otp.Destination)},
                                                is_used = @{nameof(Otp.IsSent)},
                                                usage = @{nameof(Otp.Usage)},
                                                is_sent = @{nameof(Otp.IsSent)},
                                                expires_at = @{nameof(Otp.ExpiresAt)},
                                                create_timestamp = @{nameof(Otp.CreateTimestamp)},
                                                update_timestamp = @{nameof(Otp.UpdateTimestamp)} 
                                        WHERE id = @{nameof(Otp.Id)};";

    public const string DeleteQuery = @$"";

    public const string GetByIdQuery = @$"SELECT
                                            id as {nameof(Otp.Id)},
                                            code as {nameof(Otp.Code)},
                                            is_used as {nameof(Otp.IsUsed)},
                                            usage as {nameof(Otp.Usage)},
                                            is_sent as  {nameof(Otp.IsSent)},
                                            destination as {nameof(Otp.Destination)},
                                            expires_at as {nameof(Otp.ExpiresAt)},
                                            create_timestamp as {nameof(Otp.CreateTimestamp)},
                                            update_timestamp as {nameof(Otp.UpdateTimestamp)},
                                        FROM otps WHERE id = @id LIMIT 1";
}