import hedgehog from "../assets/hedgehog.png";
import Tile from "./Tile";

export default function LogoTile({ children, className = "" }) {
  return (
    <Tile className="container mx-auto mt-16">
      <div className="container flex flex-col items-center sm:flex-row sm:items-end gap-8 p-4">
        <img
          src={hedgehog}
          alt="Hedgehog"
          className="hedgehog-logo"
          width="256"
        />
        <h1 className="text-6xl font-bold">TradeHub</h1>
      </div>

      <div className={`container flex p-4 ${className}`}>{children}</div>
    </Tile>
  );
}
