import hedgehog from "../assets/logo.png";
import Tile from "./Tile";

export default function LogoTile({ children, className = "" }) {
  return (
    <Tile className="container mx-auto mt-12" style={{ position: 'relative', backgroundImage: 'url(pexels-quintingellar-696205.jpg)', backgroundSize: 'cover', backgroundPosition: 'center' }}>
      <div className="container flex flex-col items-center sm:flex-row sm:items-end gap-8 p-4">
        <img
          src={hedgehog}
          alt="TradeHub Hedgehog"
          className="drop-shadow-lg"
          width="128"
        />
        <h1 className="text-6xl font-bold text-white drop-shadow-lg">TradeHub</h1>
      </div>

      <div className={`container flex p-4 ${className}`}>{children}</div>
    </Tile>
  );
}
