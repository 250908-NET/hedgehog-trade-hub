export default function HomePage() {
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
    <div>
      content
    </div>
    </>
  );
}
