import { useState, useEffect } from "react";

function App() {
  const [contacts, setContacts] = useState([]);
  const [newContact, setNewContact] = useState({
    id: null,
    name: "",
    email: "",
    address: "",
    phoneNumbers: [],
    imageUrl: "",
  });
  const [editingContactId, setEditingContactId] = useState(null); // Stores actual ID for API
  const [editIndex, setEditIndex] = useState(null); // Stores array index for UI, if needed

  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const API_BASE_URL = "https://localhost:7192/api"; // Ensure your API backend runs on this port

  // Function to clear error messages
  const clearErrorMessage = () => setErrorMessage("");

  // Fetch contacts from API
  const fetchContacts = async () => {
    setLoading(true);
    clearErrorMessage();
    try {
      const response = await fetch(`${API_BASE_URL}/contacts`);
 
      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP error! status: ${response.status}, details: ${errorText}`);
      }

      let data = await response.json();
      console.log(data);
      if (data.hasOwnProperty('$values')) { // Handle .$values wrapper
        data = data.$values;
      }
      const transformedContacts = data.map(contact => ({
        id: contact.Id,
        name: contact.Name,
        address: contact.Address,
        phoneNumbers: contact.PhoneNumbers ? contact.PhoneNumbers.map(pn => pn.Number) : [],
      }));
      setContacts(transformedContacts);
    } catch (error) {
      console.error("Could not fetch contacts:", error);
      setErrorMessage(`Failed to fetch contacts: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchContacts();
  }, []); // Initial fetch on component mount

  const handleInputChange = (e) => {
    setNewContact({ ...newContact, [e.target.name]: e.target.value });
  };

  const handlePhoneChange = (e, index) => {
    const updatedPhoneNumbers = [...newContact.phoneNumbers];
    updatedPhoneNumbers[index] = e.target.value;
    setNewContact({ ...newContact, phoneNumbers: updatedPhoneNumbers });
  };

  const addPhoneNumber = () => {
    setNewContact({ ...newContact, phoneNumbers: [...newContact.phoneNumbers, ""] });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    clearErrorMessage();

    const payload = {
      Name: newContact.name,
      Email: newContact.email, // Ensure backend ContactRequestDto has Email
      Address: newContact.address,
      PhoneNumbers: newContact.phoneNumbers.map(numStr => ({ number: numStr })).filter(pn => pn.number && pn.number.trim() !== ""),
      // Photo/imageUrl is not sent here; backend uses PhotoUrl from a separate upload mechanism.
    };

    let url = `${API_BASE_URL}/contacts`;
    let method = 'POST';

    if (editingContactId !== null) {
      url = `${API_BASE_URL}/contacts/${editingContactId}`;
      method = 'PUT';
    }

    try {
      const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP error! status: ${response.status}, details: ${errorText}`);
      }

      setNewContact({ id: null, name: "", email: "", address: "", phoneNumbers: [], imageUrl: "" });
      setEditingContactId(null);
      setEditIndex(null);
      await fetchContacts(); // Refresh list
    } catch (error) {
      console.error("Failed to save contact:", error);
      setErrorMessage(`Failed to save contact: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };

  const editContact = (index) => {
    const contactToEdit = contacts[index];
    setNewContact({ ...contactToEdit }); // Load contact data into form
    setEditingContactId(contactToEdit.id);
    setEditIndex(index); // Keep track of the original index if needed for UI
    clearErrorMessage();
  };

  const deleteContact = async (index) => {
    const contactToDelete = contacts[index];
    if (!contactToDelete || typeof contactToDelete.id === 'undefined') {
      setErrorMessage("Cannot delete contact: ID is missing.");
      return;
    }
    
    if (!window.confirm(`Are you sure you want to delete ${contactToDelete.name}?`)) {
        return;
    }

    setLoading(true);
    clearErrorMessage();
    try {
      const response = await fetch(`${API_BASE_URL}/contacts/${contactToDelete.id}`, {
        method: 'DELETE',
      });
      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP error! status: ${response.status}, details: ${errorText}`);
      }
      await fetchContacts(); // Refresh list
    } catch (error) {
      console.error("Failed to delete contact:", error);
      setErrorMessage(`Failed to delete contact: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };
  
  // mergeContacts remains client-side as per earlier decision
  const mergeContacts = (index1, index2) => {
    if (index1 < 0 || index1 >= contacts.length || index2 < 0 || index2 >= contacts.length || index1 === index2) {
      setErrorMessage("Invalid selection for merging contacts.");
      return;
    }
    const contact1 = contacts[index1];
    const contact2 = contacts[index2];

    const mergedPhoneNumbers = [...new Set([...contact1.phoneNumbers, ...contact2.phoneNumbers])];
    const mergedContact = {
      id: `merged_${Date.now()}`, // Temporary client-side ID
      name: contact1.name || contact2.name,
      email: contact1.email || contact2.email,
      address: contact1.address || contact2.address,
      phoneNumbers: mergedPhoneNumbers,
      imageUrl: contact1.imageUrl || contact2.imageUrl || "https://via.placeholder.com/150",
    };
    const updatedContacts = contacts.filter((_, index) => index !== index1 && index !== index2);
    setContacts([...updatedContacts, mergedContact].sort((a,b) => a.name.localeCompare(b.name)));
    setErrorMessage("Contacts merged locally. This change is not saved to the server.");
  };


  return (
    <div className="min-h-screen bg-gray-100 p-4">
      <div className="max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold text-center text-blue-600 mb-6">Contact Management</h1>

        {errorMessage && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-4" role="alert">
            <strong className="font-bold">Error: </strong>
            <span className="block sm:inline">{errorMessage}</span>
            <button onClick={clearErrorMessage} className="absolute top-0 bottom-0 right-0 px-4 py-3">
              <svg className="fill-current h-6 w-6 text-red-500" role="button" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"><title>Close</title><path d="M14.348 14.849a1.2 1.2 0 0 1-1.697 0L10 11.819l-2.651 3.029a1.2 1.2 0 1 1-1.697-1.697l2.758-3.15-2.759-3.152a1.2 1.2 0 1 1 1.697-1.697L10 8.183l2.651-3.031a1.2 1.2 0 1 1 1.697 1.697l-2.758 3.152 2.758 3.15a1.2 1.2 0 0 1 0 1.698z"/></svg>
            </button>
          </div>
        )}

        {loading && <p className="text-center text-blue-500 text-xl my-4">Loading...</p>}

        <div className="bg-white p-6 rounded-lg shadow-xl mb-8">
          <h2 className="text-2xl font-semibold mb-4 text-gray-700">{editingContactId !== null ? "Edit Contact" : "Add New Contact"}</h2>
          <form onSubmit={handleSubmit}>
            {/* Input fields */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
              <input type="text" name="name" placeholder="Name (Required)" value={newContact.name} onChange={handleInputChange} required className="input-style" />
              <input type="email" name="email" placeholder="Email" value={newContact.email} onChange={handleInputChange} className="input-style" />
              <input type="text" name="address" placeholder="Address" value={newContact.address} onChange={handleInputChange} className="input-style" />
              <input type="text" name="imageUrl" placeholder="Image URL (for display)" value={newContact.imageUrl} onChange={handleInputChange} className="input-style" aria-describedby="imageUrlHelp"/>
            </div>
            <p id="imageUrlHelp" className="text-xs text-gray-500 mb-4 -mt-3">Note: For displaying an image from a URL. Actual image uploads need a different setup.</p>

            <h3 className="text-lg font-medium text-gray-600 mb-2">Phone Numbers</h3>
            {newContact.phoneNumbers.map((phone, index) => (
              <div key={index} className="flex mb-2 items-center">
                <input type="tel" value={phone} onChange={(e) => handlePhoneChange(e, index)} placeholder="Phone Number" className="input-style flex-grow"/>
              </div>
            ))}
            <button type="button" onClick={addPhoneNumber} disabled={loading} className="btn-secondary mr-2">Add Phone</button>
            <button type="submit" disabled={loading} className="btn-primary">
              {editingContactId !== null ? "Save Changes" : "Add Contact"}
            </button>
             {editingContactId !== null && (
                <button type="button" onClick={() => { setEditingContactId(null); setNewContact({ id: null, name: "", email: "", address: "", phoneNumbers: [], imageUrl: "" }); clearErrorMessage();}} disabled={loading} className="btn-neutral ml-2">
                    Cancel Edit
                </button>
            )}
          </form>
        </div>
        
        <div className="flex justify-between items-center mb-4">
            <h2 className="text-2xl font-semibold text-gray-700">Contacts List</h2>
            <button onClick={fetchContacts} disabled={loading} className="btn-secondary">
              Refresh Contacts
            </button>
        </div>

        <ul className="space-y-4">
          {contacts.map((contact, index) => (
            <li key={contact.id} className="bg-white p-5 rounded-lg shadow-lg hover:shadow-xl transition-shadow duration-300">
              <div className="flex items-start space-x-4">
                {/* <img src={contact.imageUrl} alt={contact.name} className="w-20 h-20 rounded-full border-2 border-blue-300 object-cover shadow" onError={(e) => { e.target.src = 'https://via.placeholder.com/150'; }}/> */}
                <div className="flex-grow">
                  <h3 className="text-xl font-semibold text-blue-700">{contact.name}</h3>
                  {/* {contact.email && <p className="text-gray-600">{contact.email}</p>} */}
                  {contact.address && <p className="text-gray-500 text-sm">{contact.address}</p>}
                  {contact.phoneNumbers && contact.phoneNumbers.length > 0 && (
                    <div className="mt-2">
                      <h4 className="font-medium text-xs text-gray-500">Phones:</h4>
                      <ul className="list-disc list-inside ml-1">
                        {contact.phoneNumbers.map((phone, phoneIdx) => <li key={phoneIdx} className="text-gray-600 text-sm">{phone}</li>)}
                      </ul>
                    </div>
                  )}
                </div>
                <div className="flex flex-col space-y-2 items-end">
                  <button onClick={() => editContact(index)} disabled={loading} className="btn-edit text-xs py-1 px-2">Edit</button>
                  <button onClick={() => deleteContact(index)} disabled={loading} className="btn-delete text-xs py-1 px-2">Delete</button>
                  {index + 1 < contacts.length && (
                    <button onClick={() => mergeContacts(index, index + 1)} disabled={loading} className="btn-neutral text-xs py-1 px-2">Merge Next</button>
                  )}
                </div>
              </div>
            </li>
          ))}
        </ul>
        {!contacts.length && !loading && <p className="text-center text-gray-500 mt-6">No contacts found. Try adding some!</p>}
      </div>
    </div>
  );
}

// Basic styling (can be moved to index.css)
const styles = `
.input-style {
  display: block;
  width: 100%;
  padding: 0.75rem;
  margin-bottom: 1rem;
  border: 1px solid #D1D5DB; /* gray-300 */
  border-radius: 0.375rem; /* rounded-lg */
  box-shadow: inset 0 1px 2px 0 rgba(0, 0, 0, 0.05);
}
.input-style:focus {
  outline: none;
  border-color: #3B82F6; /* blue-500 */
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.5); /* ring-2 ring-blue-500 */
}
.btn-primary {
  background-color: #3B82F6; /* blue-500 */
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  font-weight: 500;
}
.btn-primary:hover {
  background-color: #2563EB; /* blue-600 */
}
.btn-primary:disabled {
  background-color: #9CA3AF; /* gray-400 */
  cursor: not-allowed;
}
.btn-secondary {
  background-color: #6B7280; /* gray-500 */
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  font-weight: 500;
}
.btn-secondary:hover {
  background-color: #4B5563; /* gray-600 */
}
.btn-secondary:disabled {
  background-color: #D1D5DB; /* gray-300 */
   cursor: not-allowed;
}
.btn-neutral {
  background-color: #E5E7EB; /* gray-200 */
  color: #374151; /* gray-700 */
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  border: 1px solid #D1D5DB;
}
.btn-neutral:hover {
  background-color: #D1D5DB; /* gray-300 */
}
.btn-neutral:disabled {
   background-color: #F3F4F6; /* gray-100 */
   color: #9CA3AF; /* gray-400 */
   cursor: not-allowed;
}
.btn-edit { background-color: #F59E0B; /* amber-500 */ color: white; }
.btn-edit:hover { background-color: #D97706; /* amber-600 */ }
.btn-delete { background-color: #EF4444; /* red-500 */ color: white; }
.btn-delete:hover { background-color: #DC2626; /* red-600 */ }

/* Add this to your index.css or a <style> tag in index.html if you prefer */
/* For Tailwind-like utility classes, you'd typically have Tailwind CSS setup */
/* This is a very minimal stand-in for some button/input styles */
`;

// Inject styles (for demonstration if not using Tailwind or separate CSS file)
// In a real app, put this in index.css and import it.
const styleSheet = document.createElement("style");
styleSheet.type = "text/css";
styleSheet.innerText = styles;
document.head.appendChild(styleSheet);


export default App;