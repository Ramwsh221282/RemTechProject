using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace RemTech.Infrastructure.PostgreSql.NpgSqlCustomQueryParameters;

public class JsonBTypeParameter<T>(T value) : SqlMapper.ICustomQueryParameter
{
    private readonly T _value = value;

    public void AddParameter(IDbCommand command, string name)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = JsonSerializer.Serialize(_value);
        parameter.DbType = DbType.String;
        command.Parameters.Add(parameter);
        ((NpgsqlParameter)parameter).NpgsqlDbType = NpgsqlDbType.Jsonb;
    }
}
