# StorageChest

## Quickstart
Extract in your games root folder(where the exe is) and run the game once to create folders and config files.
Drop all PNG files into the Plugins/Sprites folder and you're good to go.


Two folders will be created, the "Sprites" folder and the "SpriteData" folder. Easiest thing to do is to just put all your PNG files into the Sprites folder. The SpriteData folder allows you to create JSON files to set the properties for each image. The JSON structure looks like: 
```
{
  "SpriteData": [
    {
      "SpritePath": "C:\\path\\to\\your\\game\\Femboy Survival Demo 14\\BepInEx\\plugins\\Sprites\\-1464723533.png",
      "HypnoTypes": [
		"Anal",
		"Oral",
		"Sissy",
		"Cum",
		"Masturbation",
		"Bimbo",
		"Feet"
      ],
      "XpGiven": 1
    }
  ]
}
```
Making the JSON files is not required. You can just simply drop the images in the Sprites folder. However, this will give the images all of the hypnotypes, which might matter, or might not.
