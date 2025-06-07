using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneBook.DTOs;
using PhoneBook.Helpers;
using PhoneBook.Repository;
using System.Security.Claims;
using FileServ.Controllers.Helpers;
using PhoneBook.Helpers;

namespace WebHomework.Controllers
{
    [Route("api/contacts")]
    [ApiController]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;

        public ContactsController(IContactRepository contactRepository)
        {
            this._contactRepository = contactRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ContactResponseDto>>> GetContacts()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                return Ok(await _contactRepository.GetContacts(userId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactResponseDto>> GetContact(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var contact = await _contactRepository.GetContact(userId, id);

                if (contact == null)
                {
                    return NotFound();
                }

                return contact;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ContactResponseDto>>> SearchContacts([FromQuery] string searchTerm)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                int limit = 5; // Define a default limit, e.g., 5 or 10

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return Ok(new List<ContactResponseDto>()); // Return empty list if search term is empty
                }

                var contacts = await _contactRepository.SearchContactsAsync(userId, searchTerm, limit);
                return Ok(contacts);
            }
            catch (Exception)
            {
                // Consider more specific error logging if available
                return StatusCode(StatusCodes.Status500InternalServerError, "Error performing search.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContactResponseDto>> AddContact(ContactRequestDto contactDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var result = await _contactRepository.AddContact(userId, contactDto);
                if (!result.Success)
                {
                    if (result.Error == "Name field cannot be empty!")
                    {
                        return Conflict(result.Error);
                    }
                    if (result.Error == $"Cannot have contact with same name {contactDto.Name}.")
                    {
                        return Conflict(result.Error);
                    }
                    if (result.Error == $"Contact with id {result.Data?.Id} already exists!")
                    {
                        return Conflict(result.Error);
                    }
                }

                return CreatedAtAction(nameof(GetContact), new { id = result.Data?.Id }, result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPost("{id}/phonenumbers")]
        public async Task<ActionResult<PhoneNumberResponseDto>> AddPhoneToContact(int id, PhoneNumberRequestDto phoneNumberDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var result = await _contactRepository.AddPhoneToContact(userId, id, phoneNumberDto);
                if (!result.Success)
                {
                    if (result.Error == $"Contact with id {id} was not found!")
                    {
                        return NotFound(result.Error);
                    }
                    if (result.Error == $"Phone number {phoneNumberDto.Number} already exists!")
                    {
                        return NotFound(result.Error);
                    }

                    return BadRequest(result.Error);
                }

                return CreatedAtAction(nameof(GetContact), new { id = result.Data?.Id }, result.Data);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ContactResponseDto>> DeleteContact(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var contact = await _contactRepository.DeleteContact(userId, id);

                if (contact == null)
                {
                    return NotFound($"Contact with id {id} doesn't exist!");
                }

                return contact;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpDelete("{id}/phonenumbers/{phoneId}")]
        public async Task<ActionResult<PhoneNumberResponseDto>> DeletePhoneNumberFromContact(int id, int phoneId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var result = await _contactRepository.DeletePhoneNumberFromContact(userId, id, phoneId);

                if (!result.Success)
                {
                    if (result.Error == $"Contact with id {id} was not found!")
                    {
                        return NotFound(result.Error);
                    }
                    else if (result.Error == $"Phone number with id {phoneId} was not found!")
                    {
                        return NotFound(result.Error);
                    }

                    return BadRequest(result.Error);
                }

                return Ok(result.Data);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ContactResponseDto>> UpdateContact([FromRoute] int id, [FromBody] ContactRequestDto updatedContactDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var contact = await _contactRepository.UpdateContact(userId, id, updatedContactDto);

                if (contact == null)
                {
                    return NotFound();
                }

                return contact;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPatch("{id}/phonenumbers/{phoneId}")]
        public async Task<ActionResult<PhoneNumberResponseDto>> UpdatePhoneNumberInContact(int id, int phoneId, PhoneNumberRequestDto phoneNumberDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var result = await _contactRepository.UpdatePhoneNumberInContact(userId, id, phoneId, phoneNumberDto);

                if (!result.Success)
                {
                    if (result.Error == $"Contact with id {id} was not found!")
                    {
                        return NotFound(result.Error);
                    }
                    else if (result.Error == $"Phone with id {phoneId} was not found!")
                    {
                        return NotFound(result.Error);
                    }

                    return BadRequest(result.Error);
                }

                return Ok(result.Data);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPost("{id}/photo")]
        public async Task<IActionResult> AddPhotoToContact(int id, IFormFile file)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (file == null || file.Length == 0)
                {
                    return BadRequest("Upload a valid file.");
                }

                var uploadHandler = new UploadHandler();
                var fileName = await uploadHandler.Upload(file);

                if (!GenericHelpers.IsValidImageFileName(fileName))
                {
                    // UploadHandler.Upload now returns error messages directly if validation fails.
                    // IsValidImageFileName is a secondary check for controller-side logic if needed,
                    // but the primary error comes from uploadHandler.Upload
                    return BadRequest(fileName);
                }

                var contactDto = await _contactRepository.AddPhotoToContact(userId, id, fileName);

                if (contactDto == null)
                {
                    return NotFound($"Contact with id {id} not found.");
                }

                return Ok(contactDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading photo: {ex.Message}");
            }
        }



        [HttpDelete("{id}/photo")]
        public async Task<IActionResult> DeletePhotoFromContact(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var success = await _contactRepository.DeletePhotoFromContact(userId, id);

                if (!success)
                {
                    return NotFound($"Photo or contact with id {id} not found.");
                }

                return Ok("Photo deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting photo: {ex.Message}");
            }
        }

        [HttpGet("{id}/photo")]
        public IActionResult GetPhotoFromContact(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // You might retrieve the file name from DB based on userId and contactId
                var photoFileName = _contactRepository.GetPhotoFileName(userId, id);

                if (string.IsNullOrEmpty(photoFileName))
                {
                    return NotFound("Photo not found.");
                }

                var filePath = Path.Combine("Uploads", photoFileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Photo file not found on server.");
                }

                var imageBytes = System.IO.File.ReadAllBytes(filePath);
                var contentType = GenericHelpers.GetContentType(filePath); // Detect image MIME type based on file extension

                return File(imageBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving photo: {ex.Message}");
            }
        }



    }
}
