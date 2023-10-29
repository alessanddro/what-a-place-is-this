using Microsoft.AspNetCore.Mvc;
using what_a_place_is_this.api.Models;
using what_a_place_is_this.api.Services;

namespace what_a_place_is_this.api.Controllers;

[ApiController]
[Route("api/places")]
public class PlaceController : ControllerBase
{
    private readonly PlaceService _service;
    private readonly string _HOST;

    public PlaceController(PlaceService service, IConfiguration config)
    {
        _service = service;
        _HOST = config["Host"];
    }

    [HttpGet]
    public async Task<List<Place>> Get()
    {
        List<Place> place = await _service.GetAsync();
        foreach (var item in place)
        {
            for (int i = 0; i < item.Pictures.Count; i++)
            {
                if (item.Pictures[i].Validated == true)
                {
                    Console.WriteLine("Path: " + _HOST);
                    if (item.Pictures[i].Active == true)
                    {
                        item.Pictures[i].Path = _HOST + item.Pictures[i].Path.Replace("wwwroot/", "");
                    }
                    else
                    {
                        item.Pictures.RemoveAt(i);
                    }
                }
                else
                {
                    item.Pictures.RemoveAt(i);
                }
            }
        }
        return place;
    }
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Place>> Get(string id)
    {
        Place place = await _service.GetAsync(id);

        if (place is null)
        {
            return NotFound();
        }
        for (int i = 0; i < place.Pictures.Count; i++)
        {
            if (place.Pictures[i].Validated == true)
            {
                if (place.Pictures[i].Active == true)
                {
                    place.Pictures[i].Path = _HOST + place.Pictures[i].Path.Replace("wwwroot/", "");
                }
                else
                {
                    place.Pictures.RemoveAt(i);
                }
            }
            else
            {
                place.Pictures.RemoveAt(i);
            }
        }
        return place;
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromForm] Place newPlace, IFormFile[] picture)
    {
        Picture newPicture = new Picture();
        for (int i = 0; i < picture.Length; i++)
        {
            var ext = Path.GetExtension(picture[i].FileName);
            string uuid = Guid.NewGuid().ToString();
            string filePath = Path.Combine("wwwroot/Storage/PlacePictures/", uuid + ext);

            using Stream fileStream = new FileStream(filePath, FileMode.Create);
            picture[i].CopyTo(fileStream);
            newPicture.Path = filePath;
            newPlace?.Pictures?.Add(newPicture);
        }
        await _service.CreateAsync(newPlace);
        return CreatedAtAction(nameof(Get), new { id = newPlace.Id }, newPlace);
    }

    [HttpPatch("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, [FromBody] Place updatedBook)
    {
        var place = await _service.GetAsync(id);

        if (place is null)
        {
            return NotFound();
        }

        updatedBook.Id = place.Id;

        await _service.UpdateAsync(id, updatedBook);

        return NoContent();
    }

    [HttpGet("qr/{id:length(24)}")]
    public Task<IActionResult> GenerateQRCode(string id)
    {
        return Task.FromResult<IActionResult>(Ok(_service.GenerateQRCode(id)));
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var place = await _service.GetAsync(id);

        if (place is null)
        {
            return NotFound();
        }

        await _service.RemoveAsync(id);

        return NoContent();
    }
}
