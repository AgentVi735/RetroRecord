from PIL import Image, ImageDraw
import math

# ============================================
# ADJUSTABLE SETTINGS
# ============================================
size = 4096

num_tracks = 7                 # 1–7 (ignored if maxi single)
is_maxi_single = False

# Lead-in / Lead-out
lead_in_mm = 2.8
lead_in_spacing = 0.7
lead_out_spacing = 6.0

# Music tracks
track_groove_spacing = 0.28
gap_width = 0.8

# Realistic limits
outer_music_r = 146.0
max_music_inner_r = 52.0
inner_empty = 12.0

# ============================================
# CODE
# ============================================

img = Image.new('L', (size, size), 0)
draw = ImageDraw.Draw(img)
cx = cy = size // 2

physical_radius_mm = 150.0
pixel_radius = size // 2 - 30
mm_to_px = pixel_radius / physical_radius_mm

def r_mm_to_px(r_mm):
    return r_mm * mm_to_px

def draw_spiral(r_start, r_end, spacing_mm, line_width=1):
    if r_start <= r_end + 0.05:
        return
    b = spacing_mm / (2 * math.pi)
    theta = r_start / b
    theta_end = max(r_end / b, 0.1)
    points = []
    steps = max(80, int((theta - theta_end) * 90))
    for i in range(steps + 1):
        t = theta - (theta - theta_end) * (i / steps)
        r = b * t
        x = cx + r_mm_to_px(r) * math.cos(t)
        y = cy + r_mm_to_px(r) * math.sin(t)
        points.append((x, y))
    if len(points) > 1:
        draw.line(points, fill=255, width=line_width)

def draw_dense_track(r_start, r_end, spacing):
    r = r_start
    while r > r_end + 0.01:
        r_px = r_mm_to_px(r)
        bbox = [cx - r_px, cy - r_px, cx + r_px, cy + r_px]
        draw.ellipse(bbox, outline=255, width=1)
        r -= spacing

# ---------- Maxi Single ----------
if is_maxi_single:
    print("Mode: Maxi Single (1 track = outer 2/5)")
    lead_in_end = outer_music_r - lead_in_mm
    track_end_r = outer_music_r * (1 - 2/5)
    
    draw_spiral(outer_music_r, lead_in_end, lead_in_spacing)
    draw_dense_track(lead_in_end, track_end_r, track_groove_spacing)
    draw_spiral(track_end_r, inner_empty, lead_out_spacing)

# ---------- Realistic multi-track ----------
else:
    num_tracks = max(1, min(7, num_tracks))
    print(f"Mode: Realistic LP — {num_tracks} tracks")
    
    lead_in_end = outer_music_r - lead_in_mm
    available = lead_in_end - max_music_inner_r
    
    base_width = 11.8
    if num_tracks == 7:
        base_width = 9.2
    elif num_tracks <= 3:
        base_width = 11.0
    
    # More random-looking variation (no alternating pattern)
    variations = [1.15, 0.78, 1.05, 1.25, 0.88, 0.95, 1.12]
    
    track_widths = []
    for i in range(num_tracks):
        w = base_width * variations[i % len(variations)]
        track_widths.append(w)
    
    total_tracks = sum(track_widths)
    total_gaps = (num_tracks - 1) * gap_width
    needed = total_tracks + total_gaps
    
    if needed > available:
        scale = available / needed
        track_widths = [w * scale for w in track_widths]
        print(f"Scaled track widths by {scale:.2f} to stay within limit")
    
    print("Track widths (mm):", [f"{w:.1f}" for w in track_widths])
    
    draw_spiral(outer_music_r, lead_in_end, lead_in_spacing)
    
    current_r = lead_in_end
    for t in range(num_tracks):
        tw = track_widths[t]
        draw_dense_track(current_r, current_r - tw, track_groove_spacing)
        current_r -= tw
        
        if t < num_tracks - 1:
            gap_end = current_r - gap_width
            r = current_r - 0.15
            while r > gap_end + 0.05:
                r_px = r_mm_to_px(r)
                bbox = [cx - r_px, cy - r_px, cx + r_px, cy + r_px]
                draw.ellipse(bbox, outline=255, width=1)
                r -= 1.8
            current_r = gap_end
    
    draw_spiral(current_r, inner_empty, lead_out_spacing)

# Outer rim
draw.ellipse([cx - pixel_radius, cy - pixel_radius,
              cx + pixel_radius, cy + pixel_radius], outline=255, width=2)

img.save('groove_texture.png')
print("Saved groove_texture.png")