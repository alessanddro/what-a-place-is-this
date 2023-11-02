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
    private readonly IMongoCollection<PlaceModel> _placeCollection;

    public PlaceService(IOptions<DatabaseSettings> bookStoreDatabaseSettings)
    {
        _conn = new Conn(bookStoreDatabaseSettings);
        var _dataBase = _conn.getDataBase();

        _placeCollection = _dataBase.GetCollection<PlaceModel>("Places");
    }

    public async Task<List<PlaceModel>> GetAsync()
    {
        return await _placeCollection.Find(_ => true).ToListAsync();
    }

    public async Task<PlaceModel?> GetAsync(string id)
    {
        return await _placeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public Object GenerateQRCode(string id)
    {
        string qrcode = "https://chart.googleapis.com/chart?chs=512x512&cht=qr&chl=" + id;
        var response = new
        {
            qrcurl = qrcode
        };
        return response;
    }

    public async Task CreateAsync(PlaceModel newPlace)
    {
        await _placeCollection.InsertOneAsync(newPlace);
    }

    public async Task UpdateAsync(string id, [FromBody] PlaceModel updatedPlace)
    {
        await _placeCollection.ReplaceOneAsync(x => x.Id == id, updatedPlace);
    }

    public async Task AddComment(PlaceModel place, [FromBody] Comments comment)
    {
        place.Comment.Add(comment);
        await _placeCollection.ReplaceOneAsync(x => x.Id == place.Id, place);
    }

    public async Task AddPicture(PlaceModel place, [FromBody] List<Picture> pictures)
    {
        foreach (var pic in pictures)
        {
            place.Pictures.Add(pic);
        }

        await _placeCollection.ReplaceOneAsync(x => x.Id == place.Id, place);
    }

    public async Task RemoveAsync(string id)
    {
        await _placeCollection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task RemoveCommentAsync(string commentId, PlaceModel place)
    {
        for (int i = 0; i < place.Comment.Count; i++)
        {
            if (place.Comment[i].Id == commentId)
            {
                place.Comment.RemoveAt(i);
            }
        }
        await _placeCollection.ReplaceOneAsync(x => x.Id == place.Id, place);
    }

    public async Task RemovePictureAsync(string pictureId, PlaceModel place)
    {
        for (int i = 0; i < place.Pictures.Count; i++)
        {
            if (place.Pictures[i].Id == pictureId)
            {
                place.Pictures.RemoveAt(i);
            }
        }
        await _placeCollection.ReplaceOneAsync(x => x.Id == place.Id, place);
    }

    public async Task UpdateEvaluation(PlaceModel place, string userId)
    {
        var evaluation = place.Evaluation;
        Predicate<string> find = item => item == userId;

        if (evaluation[0].Contains(userId))
        {
            evaluation[0].Remove(userId);
            evaluation[1] = evaluation[1] - 1;
            Console.WriteLine("Usuario, " + userId + " removido");
            Console.WriteLine("Total, " + evaluation[1]);
        }
        else
        {
            evaluation[0].Add(userId);
            evaluation[1] = evaluation[0].Count;
            Console.WriteLine("Usuario, " + userId + " adicionado");
        }
        place.Evaluation = evaluation;
        await _placeCollection.ReplaceOneAsync(x => x.Id == place.Id, place);
    }
}
