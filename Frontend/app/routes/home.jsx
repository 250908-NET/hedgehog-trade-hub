import { useState, useEffect } from 'react';

// Item card component builder
function ItemCard({ item }) {
  return (
    <div style={{
      padding: '10px',
      borderRadius: '5px',
      boxShadow: '2px 2px 5px rgba(0,0,0,0.1)',
      maxWidth: 'fit-content',
      backgroundColor: 'white'
    }}>
      <h2 style={{ margin: '5px 0' }}>{item.name}</h2>
      <div style={{
        fontSize: '14px',
        color: '#555',
        border: '1px solid #000000',
        padding: '10px',
        margin: '5px 0'
      }}>
        <img 
          src={item.image || 'https://clipart-library.com/8300/2368/black-open-box-isolated-white_255502-165.jpg'} 
          alt={item.name}
          style={{
            maxWidth: '100px',
            height: 'auto',
            display: 'block',
            marginTop: '10px'
          }}
        />
        {item.description}
      </div>
      <p style={{
        fontWeight: 'bold',
        color: '#000',
        textAlign: 'center',
        margin: '5px 0'
      }} className='priceTag'>
        est. ${item.value}
      </p>
    </div>
  );
}

export default function HomePage() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Fetch items from API
  useEffect(() => {
    const fetchItems = async () => {
      setLoading(true);
      setError(null);
      
      try {
        const response = await fetch('http://localhost:5047/Items?pageSize=20');
        
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data = await response.json();
        setItems(data);
      } catch (err) {
        setError(err.message);
        console.error('Error fetching items:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchItems();
  }, []);

  return (
    <>
    <div style={{ 
        position: 'relative',
        backgroundImage: 'url(pexels-quintingellar-696205.jpg)', 
        backgroundSize: 'cover', 
        backgroundPosition: 'center' 
      }}>
        {/* Black overlay */}
        <div style={{
          position: 'absolute',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          backgroundColor: 'rgba(0, 0, 0, 0.4)',
          zIndex: 1
        }}></div>
        <div style={{ 
          position: 'relative',
          zIndex: 2,
          height: '40vh', 
          display: 'flex', 
          flexDirection: 'column', 
          justifyContent: 'center', 
          alignItems: 'center', 
          color: 'white', 
          textShadow: '2px 2px 4px rgba(0, 0, 0, 0.7)' 
        }}>
          <img src="logobanner.png" alt="Hedgehog" width={400} />
        </div>
    </div>
    
    {/* Items Container */}
    <div style={{
      display: 'flex',
      flexDirection: 'grid',
      gap: '10px',
      padding: '20px',
      backgroundColor: '#f4f4f4',
      fontFamily: 'Arial, sans-serif'
    }}>
      {loading && (
        <div style={{ textAlign: 'center', width: '100%', padding: '20px' }}>
          <p>Loading items...</p>
        </div>
      )}
      
      {error && (
        <div style={{ textAlign: 'center', width: '100%', padding: '20px', color: 'red' }}>
          <p>Error: {error}</p>
        </div>
      )}
      
      {!loading && !error && items.length === 0 && (
        <div style={{ textAlign: 'center', width: '100%', padding: '20px' }}>
          <p>No items found.</p>
        </div>
      )}
      
      {!loading && !error && items.map((item) => (
        <ItemCard key={item.id} item={item} />
      ))}
    </div>
    </>
  );
}
