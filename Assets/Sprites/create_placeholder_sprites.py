#!/usr/bin/env python3
"""
Create placeholder sprite PNG files for all game categories.
This allows development/testing to proceed without external downloads.
In production, replace with actual sprites from Kenney.nl & Game-Icons.net
"""

from PIL import Image, ImageDraw, ImageFont
import os

def create_sprite(filename, width, height, label, bg_color):
    """Create a simple placeholder PNG sprite with label."""
    img = Image.new('RGBA', (width, height), bg_color)
    draw = ImageDraw.Draw(img)
    
    # Try to use a default font
    try:
        font = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf", 12)
    except:
        font = ImageFont.load_default()
    
    # Draw border
    draw.rectangle([0, 0, width-1, height-1], outline=(0, 0, 0, 255), width=2)
    
    # Draw label text centered
    bbox = draw.textbbox((0, 0), label, font=font)
    text_width = bbox[2] - bbox[0]
    text_height = bbox[3] - bbox[1]
    text_x = (width - text_width) // 2
    text_y = (height - text_height) // 2
    draw.text((text_x, text_y), label, fill=(0, 0, 0, 255), font=font)
    
    img.save(filename)
    return filename

# Define sprite categories and files to create
sprite_specs = {
    'Mascot': [
        ('character_idle.png', 64, 64, 'Idle', (100, 150, 255, 255)),
        ('character_happy.png', 64, 64, 'Happy', (100, 200, 100, 255)),
        ('character_celebrating.png', 64, 64, 'Celebr', (255, 200, 100, 255)),
        ('character_encouraging.png', 64, 64, 'Encour', (200, 100, 255, 255)),
        ('character_thinking.png', 64, 64, 'Think', (255, 150, 100, 255)),
        ('character_confused.png', 64, 64, 'Confus', (150, 150, 255, 255)),
    ],
    'Tiles': [
        ('tile_1.png', 64, 64, '1', (255, 100, 100, 255)),
        ('tile_2.png', 64, 64, '2', (255, 150, 100, 255)),
        ('tile_3.png', 64, 64, '3', (255, 200, 100, 255)),
        ('tile_4.png', 64, 64, '4', (200, 255, 100, 255)),
        ('tile_5.png', 64, 64, '5', (100, 255, 100, 255)),
        ('tile_6.png', 64, 64, '6', (100, 255, 150, 255)),
        ('tile_7.png', 64, 64, '7', (100, 255, 200, 255)),
        ('tile_8.png', 64, 64, '8', (100, 200, 255, 255)),
        ('tile_9.png', 64, 64, '9', (100, 150, 255, 255)),
        ('tile_star.png', 64, 64, '★', (255, 255, 100, 255)),
        ('tile_heart.png', 64, 64, '❤', (255, 100, 150, 255)),
        ('tile_gem.png', 64, 64, '💎', (200, 100, 255, 255)),
        ('tile_base.png', 64, 64, 'Base', (200, 200, 200, 255)),
        ('tile_selected.png', 64, 64, 'Sel', (50, 150, 50, 255)),
        ('tile_matched.png', 64, 64, 'Match', (100, 255, 100, 255)),
        ('tile_locked.png', 64, 64, 'Lock', (100, 100, 100, 255)),
    ],
    'UI': [
        ('button_play.png', 128, 64, 'Play', (100, 200, 100, 255)),
        ('button_pause.png', 128, 64, 'Pause', (200, 150, 100, 255)),
        ('button_settings.png', 128, 64, 'Settings', (100, 150, 200, 255)),
        ('button_back.png', 128, 64, 'Back', (200, 100, 100, 255)),
        ('button_next.png', 128, 64, 'Next', (100, 200, 150, 255)),
        ('button_retry.png', 128, 64, 'Retry', (200, 100, 150, 255)),
        ('button_shop.png', 128, 64, 'Shop', (255, 200, 100, 255)),
        ('button_ads.png', 128, 64, 'Watch Ad', (150, 150, 255, 255)),
        ('panel_menu.png', 320, 240, 'Menu', (220, 220, 220, 255)),
        ('panel_pause.png', 320, 240, 'Paused', (180, 180, 200, 255)),
    ],
    'Icons': [
        ('coin.png', 32, 32, 'Coin', (255, 200, 0, 255)),
        ('gem.png', 32, 32, 'Gem', (150, 100, 255, 255)),
        ('star.png', 32, 32, 'Star', (255, 255, 100, 255)),
        ('heart.png', 32, 32, 'Heart', (255, 100, 150, 255)),
        ('bolt.png', 32, 32, 'Bolt', (255, 255, 0, 255)),
        ('shield.png', 32, 32, 'Shield', (100, 150, 255, 255)),
        ('music_on.png', 32, 32, 'Music', (100, 200, 100, 255)),
        ('music_off.png', 32, 32, 'Mute', (200, 100, 100, 255)),
        ('sound_on.png', 32, 32, 'Sound', (100, 200, 100, 255)),
        ('sound_off.png', 32, 32, 'Mute', (200, 100, 100, 255)),
    ],
    'Backgrounds': [
        ('bg_menu.png', 1080, 1920, 'Menu BG', (200, 220, 255, 255)),
        ('bg_level.png', 1080, 1920, 'Level BG', (240, 240, 240, 255)),
        ('bg_shop.png', 1080, 1920, 'Shop BG', (255, 240, 200, 255)),
        ('bg_settings.png', 1080, 1920, 'Settings', (220, 240, 220, 255)),
    ],
    'Effects': [
        ('particle_star.png', 32, 32, '✨', (255, 255, 100, 255)),
        ('particle_pop.png', 32, 32, '◉', (255, 150, 100, 255)),
        ('particle_spark.png', 32, 32, '✦', (255, 200, 100, 255)),
        ('particle_dust.png', 32, 32, '◦', (150, 150, 150, 255)),
        ('explosion.png', 64, 64, '💥', (255, 100, 100, 255)),
        ('confetti.png', 64, 64, '🎊', (255, 200, 100, 255)),
    ]
}

# Create all sprites
print("Creating placeholder sprites...")
base_path = os.path.dirname(os.path.abspath(__file__))

for category, sprites in sprite_specs.items():
    category_path = os.path.join(base_path, category)
    os.makedirs(category_path, exist_ok=True)
    
    for filename, width, height, label, color in sprites:
        filepath = os.path.join(category_path, filename)
        create_sprite(filepath, width, height, label, color)
        print(f"✓ Created {category}/{filename}")

print("\n✅ All placeholder sprites created!")
print(f"Total sprites: {sum(len(v) for v in sprite_specs.values())}")
print("\nNote: These are placeholders. Replace with actual sprites from:")
print("  - Kenney.nl (https://kenney.nl/assets)")
print("  - Game-Icons.net (https://game-icons.net)")
