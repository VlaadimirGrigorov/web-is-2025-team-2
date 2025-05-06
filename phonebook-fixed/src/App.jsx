import { useState } from "react";

function App() {
  const [contacts, setContacts] = useState([]);
  const [newContact, setNewContact] = useState({
    name: "",
    email: "",
    address: "",
    phoneNumbers: [],
    imageUrl: "",
  });
  const [editIndex, setEditIndex] = useState(null);
//Тази функция обработва промените в текстовите полета за име, имейл, адрес и URL на изображението. 
  const handleInputChange = (e) => {
    setNewContact({
      ...newContact,
      [e.target.name]: e.target.value,
    });
  };
//Функцията за обработка на телефонни номера.
  const handlePhoneChange = (e, index) => {
    const phoneNumbers = [...newContact.phoneNumbers];
    phoneNumbers[index] = e.target.value;
    setNewContact({ ...newContact, phoneNumbers });
  };

  const addPhoneNumber = () => {
    setNewContact({
      ...newContact,
      phoneNumbers: [...newContact.phoneNumbers, ""],
    });
  };
//Функция за изпращане на данни, когато потребителят добавя или редактира контакт.
  const handleSubmit = (e) => {
    e.preventDefault();
    if (editIndex !== null) {
      const updatedContacts = [...contacts];
      updatedContacts[editIndex] = newContact;
      setContacts(updatedContacts);
      setEditIndex(null);
    } else {
      setContacts([...contacts, newContact]);
    }
    setNewContact({
      name: "",
      email: "",
      address: "",
      phoneNumbers: [],
      imageUrl: "",
    });
  };

  const mergeContacts = (index1, index2) => {
    const contact1 = contacts[index1];
    const contact2 = contacts[index2];

    // Мерджваме телефонните номера
    const mergedPhoneNumbers = [
      ...new Set([...contact1.phoneNumbers, ...contact2.phoneNumbers]),
    ];

    // Обединяваме всички данни, като оставяме последния, ако има различия
    const mergedContact = {
      name: contact1.name, // Може да добавим някаква логика за избор на име, ако са различни
      email: contact1.email, // Същото важи и за имейл
      address: contact1.address || contact2.address,
      phoneNumbers: mergedPhoneNumbers,
      imageUrl: contact1.imageUrl || contact2.imageUrl,
    };

    // Премахваме старите контакти и добавяме новия обединен контакт
    const updatedContacts = contacts.filter((_, index) => index !== index1 && index !== index2);
    setContacts([...updatedContacts, mergedContact]);
  };

  const editContact = (index) => {
    setNewContact(contacts[index]);
    setEditIndex(index);
  };

  const deleteContact = (index) => {
    const updatedContacts = contacts.filter((_, i) => i !== index);
    setContacts(updatedContacts);
  };

  return (
    <div className="min-h-screen bg-gray-100 p-4">
      <h1 className="text-2xl font-bold mb-4">{editIndex !== null ? "Edit Contact" : "Add New Contact"}</h1>
      <form onSubmit={handleSubmit} className="bg-white p-6 rounded-lg shadow-lg max-w-lg mx-auto">
        <input
          type="text"
          name="name"
          placeholder="Name"
          value={newContact.name}
          onChange={handleInputChange}
          className="block w-1/4 max-w-sm p-3 mb-6 border-white-5 border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <input
          type="email"
          name="email"
          placeholder="Email"
          value={newContact.email}
          onChange={handleInputChange}
          className="block w-1/4 max-w-sm p-3 mb-6 border-white-5 border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <input
          type="text"
          name="address"
          placeholder="Address"
          value={newContact.address}
          onChange={handleInputChange}
          className="block w-1/4 max-w-sm p-3 mb-6 border-white-5 border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <input
          type="text"
          name="imageUrl"
          placeholder="Image URL (optional)"
          value={newContact.imageUrl}
          onChange={handleInputChange}
          className="block w-1/4 max-w-sm p-3 mb-6 border-white-5 border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        {newContact.phoneNumbers.map((phone, index) => (
          <input
            key={index}
            type="text"
            value={phone}
            onChange={(e) => handlePhoneChange(e, index)}
            placeholder="Phone Number"
            className="block w-1/4 p-2 mb-4 border-white-5 border-gray-300 rounded"
          />
        ))}
        <button
          type="button"
          onClick={addPhoneNumber}
          className="bg-blue-500 text-white py-2 px-4 rounded mb-4"
        >
          Add Phone Number
        </button>
        <button
          type="submit"
          className="bg-green-500 text-white py-2 px-6 rounded-lg hover:bg-green-600 focus:outline-none transition duration-300"
        >
          {editIndex !== null ? "Save Changes" : "Add Contact"}
        </button>
      </form>

      <div className="mt-8">
        <h2 className="text-2xl font-bold mb-4">Contacts</h2>
        <ul>
        {contacts.map((contact, index) => (
    <li key={index} className="bg-white p-4 mb-4 rounded shadow">
      {contact.imageUrl ? (
        <img
          src={contact.imageUrl}
          alt="Profile"
          className="w-8 h-8 rounded-full border-2 border-gray-300 shadow-lg mb-4"
        />
      ) : (
        <img
          src="https://via.placeholder.com/150"
          alt="Default Profile"
          className="w-8 h-8 rounded-full border-2 border-gray-300 shadow-lg mb-4"
        />
      )}
      <h3 className="font-semibold text-lg">{contact.name}</h3>
      <p className="text-gray-600">{contact.email}</p>
      <p className="text-gray-500">{contact.address}</p>
      <ul>
        {contact.phoneNumbers.map((phone, index) => (
          <li key={index} className="text-gray-400">{phone}</li>
        ))}
      </ul>
      <div className="flex space-x-2 mt-4">
        <button
          onClick={() => editContact(index)}
          className="bg-blue-500 text-white py-2 px-4 rounded-md transition hover:bg-blue-600 duration-300"
        >
          Edit
        </button>
        <button
          onClick={() => deleteContact(index)}
          className="bg-red-500 text-white py-2 px-4 rounded-md transition hover:bg-red-600 duration-300"
        >
          Delete
        </button>
        <button
          onClick={() => mergeContacts(index, index + 1)}
          className="bg-yellow-500 text-white py-2 px-4 rounded-md transition hover:bg-yellow-600 duration-300"
        >
                  Merge
                </button>
              </div>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}

export default App;
