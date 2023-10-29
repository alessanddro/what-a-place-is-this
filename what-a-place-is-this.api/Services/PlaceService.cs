using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using what_a_place_is_this.api.Config;
using what_a_place_is_this.api.Data;
using what_a_place_is_this.api.Models;

namespace what_a_place_is_this.api.Services;

public class PlaceService
{
    private readonly Conn _conn;
    private readonly IMongoCollection<Place> _placeCollection;
    public PlaceService(IOptions<DatabaseSettings> bookStoreDatabaseSettings)
    {
        _conn = new Conn(bookStoreDatabaseSettings);
        var _dataBase = _conn.getDataBase();

        _placeCollection = _dataBase.GetCollection<Place>("Places");
    }

    public async Task<List<Place>> GetAsync() =>
        await _placeCollection.Find(_ => true).ToListAsync();

    public async Task<Place?> GetAsync(string id) =>
        await _placeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public Object GenerateQRCode(string id)
    {
        string qrcode = "https://chart.googleapis.com/chart?chs=512x512&cht=qr&chl=" + id;
        var response = new
        {
            qrcurl = qrcode
        };
        return response;
    }

    public async Task CreateAsync(Place newPlace) =>
        await _placeCollection.InsertOneAsync(newPlace);

    public async Task UpdateAsync(string id, [FromBody] Place updatedPlace) =>
    await _placeCollection.ReplaceOneAsync(x => x.Id == id, updatedPlace);

    public async Task RemoveAsync(string id) =>
        await _placeCollection.DeleteOneAsync(x => x.Id == id);
}
