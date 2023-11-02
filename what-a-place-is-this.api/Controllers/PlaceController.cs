using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using what_a_place_is_this.api.DTOs;
using what_a_place_is_this.api.Models;
using what_a_place_is_this.api.Services;

namespace what_a_place_is_this.api.Controllers;

[ApiController]
[Route("api/places")]
public class PlaceController : ControllerBase
{
    private readonly PlaceService _service;
    private readonly string _HOST;
    private readonly IMapper _mapper;

    public PlaceController(PlaceService service, IConfiguration config, IMapper mapper)
    {
        _service = service;
        _HOST = config["Host"];
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<PlaceModel>> Get()
    {
        List<PlaceModel> place = await _service.GetAsync();
        foreach (var item in place)
        {
            var newPictures = new List<Picture>();
            foreach (var picture in item.Pictures)
            {
                if (picture.Validated)
                {
                    Console.WriteLine("Path: " + _HOST);
                    if (picture.Active)
                    {
                        picture.Path = _HOST + picture.Path.Replace("wwwroot/", "");
                        newPictures.Add(picture);
                    }
                }
            }
            item.Pictures = newPictures;
        }

        return place;
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<PlaceModel>> Get(string id)
    {
        PlaceModel place = await _service.GetAsync(id);

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

    [HttpGet("evaluation/{placeId:length(24)}/{userId:length(24)}")]
    public async Task<IActionResult> SetEvaluation(string placeId, string userId)
    {
        //User user = new()
        PlaceModel place = await _service.GetAsync(placeId);
        if (place is null)
        {
            return NotFound();
        }

        await _service.UpdateEvaluation(place, userId);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] PlaceDTO newPlace, IFormFile[] picture)
    {
        Picture newPicture = new Picture();
        PlaceModel place = new();
        place = _mapper.Map<PlaceModel>(newPlace);
        for (int i = 0; i < picture.Length; i++)
        {
            var ext = Path.GetExtension(picture[i].FileName);
            string uuid = Guid.NewGuid().ToString();
            string filePath = Path.Combine("wwwroot/Storage/PlacePictures/", uuid + ext);

            using Stream fileStream = new FileStream(filePath, FileMode.Create);
            picture[i].CopyTo(fileStream);
            newPicture.Path = filePath;
            place?.Pictures?.Add(newPicture);
        }
        await _service.CreateAsync(place);
        return CreatedAtAction(nameof(Get), new { id = place.Id }, newPlace);
    }


    [HttpPatch("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, [FromBody] PlaceModel updatedBook)
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

    [HttpPatch("comment/{placeId:length(24)}")]
    public async Task<IActionResult> AddComment(string placeId, [FromBody] Comments comment)
    {
        var place = await _service.GetAsync(placeId);

        if (place is null)
        {
            return NotFound();
        }

        await _service.AddComment(place, comment);

        return NoContent();
    }

    [HttpPatch("picture/{placeId:length(24)}")]
    public async Task<IActionResult> AddPicture(string placeId, IFormFile[] picture)
    {
        var place = await _service.GetAsync(placeId);

        if (place is null)
        {
            return NotFound();
        }

        Picture _picture = new();
        List<Picture> pictures = new();
        for (int i = 0; i < picture.Length; i++)
        {
            var ext = Path.GetExtension(picture[i].FileName);
            string uuid = Guid.NewGuid().ToString();
            string filePath = Path.Combine("wwwroot/Storage/PlacePictures/", uuid + ext);

            using Stream fileStream = new FileStream(filePath, FileMode.Create);
            picture[i].CopyTo(fileStream);
            _picture.Path = filePath;
            pictures.Add(_picture);
        }

        await _service.AddPicture(place, pictures);

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

    [HttpDelete("comment/{id:length(24)}/{commentId:length(24)}")]
    public async Task<IActionResult> DeleteComment(string id, string commentId)
    {
        PlaceModel place = await _service.GetAsync(id);

        if (place is null)
        {
            return NotFound();
        }
        await _service.RemoveCommentAsync(commentId, place);

        return NoContent();
    }

    [HttpDelete("picture/{placeId:length(24)}/{pictureId:length(24)}")]
    public async Task<IActionResult> DeletePicture(string placeId, string pictureId)
    {
        PlaceModel place = await _service.GetAsync(placeId);

        if (place is null)
        {
            return NotFound();
        }
        await _service.RemovePictureAsync(pictureId, place);

        return NoContent();
    }
}
