import Tile from "./Tile";

export default function ItemTile({ item }) {
  return (
    <Tile>
      <div className="aspect-square bg-neutral-900 mb-2 rounded flex items-center justify-center">
        {item.imgURL ? (
          <img
            src={item.imgURL}
            alt={item.name}
            className="w-full h-full object-contain"
          />
        ) : (
          <p>(no image)</p>
        )}
      </div>
      <div className="flex flex-col">
        <p className="font-bold">{item.name}</p>
        <p className="text-sm text-neutral-400">{item.condition}, {item.availability}</p>
      </div>
    </Tile>
  );
}
