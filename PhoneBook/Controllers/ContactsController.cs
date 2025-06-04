using Microsoft.AspNetCore.Mvc;
using PhoneBook.DTOs;
using PhoneBook.Helpers;
using PhoneBook.Repository;

namespace WebHomework.Controllers
{
    [Route("api/contacts")]
    [ApiController]
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
                return Ok(await _contactRepository.GetContacts());
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactResponseDto>> GetContact(int id)
        {
            try
            {
                var contact = await _contactRepository.GetContact(id);

                if (contact == null)
                {
                    return NotFound();
                }

                return contact;
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContactResponseDto>> AddContact(ContactRequestDto contactDto)
        {
            try
            {
                // check the phone numbers
                foreach (var phoneNumber in contactDto.PhoneNumbers)
                {
                    if (!GenericHelpers.IsValidPhoneNumber(phoneNumber.Number))
                    {
                        return BadRequest("Invalid phone number!");
                    }
                }

                var result = await _contactRepository.AddContact(contactDto);
                if (result == null)
                {
                    return Conflict("Contact already exists!");
                }

                return CreatedAtAction(nameof(GetContact), new { id = result?.Id }, result);
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPost("{id}/phonenumbers")]
        public async Task<ActionResult<PhoneNumberResponseDto>> AddPhoneToContact(int id, PhoneNumberRequestDto phoneNumberDto)
        {
            try
            {
                if (!GenericHelpers.IsValidPhoneNumber(phoneNumberDto.Number))
                {
                    return BadRequest($"Invalid phone number!");
                }

                var result = await _contactRepository.AddPhoneToContact(id, phoneNumberDto);
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
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ContactResponseDto>> DeleteContact(int id)
        {
            try
            {
                var contact = await _contactRepository.DeleteContact(id);
                if (contact == null)
                {
                    return NotFound($"Contact with id {id} doesn't exist!");
                }

                return contact;
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpDelete("{id}/phonenumbers/{phoneId}")]
        public async Task<ActionResult<PhoneNumberResponseDto>> DeletePhoneNumberFromContact(int id, int phoneId)
        {
            try
            {
                var result = await _contactRepository.DeletePhoneNumberFromContact(id, phoneId);

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
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ContactResponseDto>> UpdateContact([FromRoute] int id, [FromBody] ContactRequestDto updatedContactDto)
        {
            try
            {
                var contact = await _contactRepository.UpdateContact(id, updatedContactDto);

                if (contact == null)
                {
                    return NotFound();
                }

                return contact;
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPatch("{id}/phonenumbers/{phoneId}")]
        public async Task<ActionResult<PhoneNumberResponseDto>> UpdatePhoneNumberInContact(int id, int phoneId, PhoneNumberRequestDto phoneNumberDto)
        {
            try
            {
                if (!GenericHelpers.IsValidPhoneNumber(phoneNumberDto.Number))
                {
                    return BadRequest($"Invalid number {phoneNumberDto.Number}!");
                }

                var result = await _contactRepository.UpdatePhoneNumberInContact(id, phoneId, phoneNumberDto);

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
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
    }
}
