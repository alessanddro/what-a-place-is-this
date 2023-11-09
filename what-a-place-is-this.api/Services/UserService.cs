using Microsoft.Extensions.Options;
using MongoDB.Driver;
using what_a_place_is_this.api.Config;
using what_a_place_is_this.api.Data;
using what_a_place_is_this.api.Models;

namespace what_a_place_is_this.api.Services;

public class UserService
{
    private readonly Conn _conn;
    private readonly IMongoCollection<UserModel> _userCollection;

    public UserService(IOptions<DatabaseSettings> userStoreDatabaseSettings)
    {
        _conn = new Conn(userStoreDatabaseSettings);
        var _dataBase = _conn.getDataBase();
        _userCollection = _dataBase.GetCollection<UserModel>("Users");
    }

    public async Task SigninAsync(UserModel user)
    {
        await _userCollection.InsertOneAsync(user);
    }

    public async Task<UserModel> Login(UserModel user)
    {
        UserModel _user = new();
        _user = await _userCollection.Find(u => u.UserName == user.UserName).FirstOrDefaultAsync();

        if (_user is not null && _user.Pass != user.Pass)
        {
            return null;
        }
        else
        {
            return _user;
        }
    }
}
