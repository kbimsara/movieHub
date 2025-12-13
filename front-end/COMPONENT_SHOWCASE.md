# 🎨 MovieHub Component Showcase

## Navigation & Layout

### 🧭 Navbar
**Location**: `src/components/layout/Navbar.tsx`

Features:
- Logo with Film icon
- Navigation links (Home, Browse, Library, Upload)
- User authentication state
- Login/Register buttons (when logged out)
- Profile, Settings, Logout buttons (when logged in)
- Admin-only upload link
- Responsive design

---

## Movie Components

### 🎬 MovieCard
**Location**: `src/components/movie/MovieCard.tsx`

Features:
- Movie poster image with hover zoom
- Quality badge (480p, 720p, 1080p, 4K)
- Rating with star icon
- Play button overlay on hover
- Duration display
- Year of release
- Genre tags (first 2)
- Progress bar (optional)
- Add to Library button
- Click to navigate to detail page

### 📊 MovieGrid
**Location**: `src/components/movie/MovieGrid.tsx`

Features:
- Responsive grid (2-6 columns based on screen size)
- Loading skeleton states
- Empty state message
- Automatic layout adjustment

### ➡️ MovieRow
**Location**: `src/components/movie/MovieRow.tsx`

Features:
- Horizontal scrollable row
- Scroll left/right buttons on hover
- Smooth scroll animation
- Section title
- Hidden scrollbar for clean look
- Progress bar support

### 🔍 SearchBar
**Location**: `src/components/movie/SearchBar.tsx`

Features:
- Keyword search input
- Advanced filters toggle
- Genre dropdown filter
- Year input filter
- Sort by dropdown (Rating, Year, Title, Views, Recently Added)
- Expandable filters section
- Real-time search

---

## Player Components

### ▶️ VideoPlayer
**Location**: `src/components/player/VideoPlayer.tsx`

Features:
- HLS video streaming
- Custom video controls:
  - Play/Pause (center & bottom)
  - Volume control with slider
  - Mute toggle
  - Progress bar with seek
  - Time display (current/duration)
  - Quality selector button
  - Subtitle selector button
  - Fullscreen toggle
- Auto-hide controls (3 seconds)
- Click video to play/pause
- Keyboard shortcuts ready
- Progress tracking callback
- Error handling

---

## Torrent Components

### 🌐 TorrentSeedWidget
**Location**: `src/components/torrent/TorrentSeedWidget.tsx`

Features:
- Start/Stop seeding buttons
- Active seeding status indicator
- Real-time statistics:
  - Upload speed (live)
  - Download speed (live)
  - Total uploaded
  - Total downloaded
  - Number of peers
  - Seeding ratio
  - Progress percentage
- Auto-refresh every 5 seconds
- Byte formatting (B, KB, MB, GB, TB)
- Speed formatting (B/s, KB/s, MB/s)
- Success/Error notifications

---

## UI Components (ShadCN)

### 🔘 Button
**Location**: `src/components/ui/button.tsx`

Variants:
- `default` - Primary action button
- `destructive` - Dangerous actions (delete, remove)
- `outline` - Secondary actions
- `secondary` - Alternative styling
- `ghost` - Minimal styling
- `link` - Link-styled button

Sizes:
- `default` - Standard size
- `sm` - Small
- `lg` - Large
- `icon` - Square icon button

### 📝 Input
**Location**: `src/components/ui/input.tsx`

Features:
- Text, email, password, number types
- Focus ring styling
- Disabled state
- Error state support
- Placeholder support

### 🏷️ Label
**Location**: `src/components/ui/label.tsx`

Features:
- Associated with form inputs
- Accessible
- Disabled state styling

### 💬 Dialog/Modal
**Location**: `src/components/ui/dialog.tsx`

Features:
- Overlay backdrop
- Centered modal
- Close on overlay click
- Animation on open/close
- Header, title, description sections
- Scrollable content

### 📊 Progress Bar
**Location**: `src/components/ui/progress.tsx`

Features:
- Percentage-based progress
- Smooth transitions
- Customizable height
- Color variants

### 🔔 Toast Notifications
**Location**: `src/components/ui/toast.tsx` & `toaster.tsx`

Features:
- Success, error, warning, info types
- Title and description
- Auto-dismiss (3 seconds)
- Positioned top-right
- Stacking support
- Swipe to dismiss

### 📑 Tabs
**Location**: `src/components/ui/tabs.tsx`

Features:
- Multiple tab panels
- Active state styling
- Keyboard navigation
- Content switching

### 🃏 Card
**Location**: `src/components/ui/card.tsx`

Features:
- Container component
- Header section
- Content section
- Title and description

### 💀 Skeleton Loader
**Location**: `src/components/ui/skeleton.tsx`

Features:
- Pulse animation
- Customizable dimensions
- Loading state placeholder

---

## Page Layouts

### 🏠 Home Page
**Location**: `src/app/page.tsx`

Sections:
1. Hero Section
   - Featured movie backdrop
   - Title and description
   - Watch Now & More Info buttons
   
2. Continue Watching Row
   - Movies with progress bars
   - Last watched first

3. Trending Now Row
   - Horizontal scroll
   - Current trending movies

4. Popular Movies Row
   - Most popular content

5. Top Rated Row
   - Highest rated movies

### 🔍 Browse Page
**Location**: `src/app/browse/page.tsx`

Layout:
- Page title
- SearchBar with filters
- MovieGrid with results
- Loading states
- Pagination ready

### 🎬 Movie Detail Page
**Location**: `src/app/movie/[id]/page.tsx`

Sections:
1. Hero Section
   - Full-screen backdrop
   - Movie poster (left)
   - Movie info (right):
     - Title
     - Year, duration, rating, quality
     - Genres
     - Description
     - Director
     - Action buttons (Watch, Add, Favorite, Download)

2. Cast Section
   - Grid of cast members
   - Photos and character names

3. Torrent Section
   - Seeding widget

4. Related Movies
   - Horizontal row

### ▶️ Watch Page
**Location**: `src/app/watch/[id]/page.tsx`

Layout:
- Full-screen black background
- Back button (top-left)
- VideoPlayer (centered)
- Movie info below player

### 📚 Library Page
**Location**: `src/app/library/page.tsx`

Tabs:
1. All Movies - Full library
2. Continue Watching - In-progress movies
3. Favorites - Favorited movies

Each tab shows MovieGrid with progress bars

### 📤 Upload Page
**Location**: `src/app/upload/page.tsx`

Sections:
1. Drag & Drop Zone
   - File type indicator
   - Upload instructions
   - Selected file display
   - Progress bar

2. Metadata Form
   - Title (required)
   - Year (required)
   - Duration (required)
   - Genres (required)
   - Description (required)
   - Cast (optional)
   - Director (optional)
   - Tags (optional)

### ⚙️ Settings Page
**Location**: `src/app/settings/page.tsx`

Tabs:
1. **Profile** - User info, sign out
2. **Playback** - Auto-play, skip intro, quality
3. **Torrent** - Active seeds, auto-seed, speed limit
4. **Appearance** - Theme selector (Light/Dark/System)

### 🔐 Auth Pages
**Locations**: 
- `src/app/auth/login/page.tsx`
- `src/app/auth/register/page.tsx`
- `src/app/auth/forgot-password/page.tsx`

Layout:
- Centered card
- Logo at top
- Form fields
- Submit button
- Link to alternate action
- Error messages
- Loading states

---

## Color Scheme

### Light Mode
- **Background**: White (#FFFFFF)
- **Foreground**: Dark Gray (#1E293B)
- **Primary**: Blue (#3B82F6)
- **Secondary**: Light Gray (#F1F5F9)
- **Destructive**: Red (#EF4444)

### Dark Mode
- **Background**: Dark Blue (#0F172A)
- **Foreground**: White (#F8FAFC)
- **Primary**: Light Blue (#60A5FA)
- **Secondary**: Dark Gray (#1E293B)
- **Destructive**: Dark Red (#7F1D1D)

---

## Icons Used (Lucide React)

- `Film` - Logo, branding
- `Home` - Home navigation
- `Search` - Search functionality
- `Library` - Library page
- `Upload` - Upload page
- `Settings` - Settings page
- `User` - Profile
- `LogOut` - Sign out
- `Play` - Video playback
- `Pause` - Pause video
- `Plus` - Add to library
- `Heart` - Favorites
- `Star` - Ratings
- `Clock` - Duration
- `Download` - Download action
- `Volume2` / `VolumeX` - Audio controls
- `Maximize` - Fullscreen
- `Subtitles` - Subtitle selector
- `ArrowLeft` - Back navigation
- `ChevronLeft` / `ChevronRight` - Scroll controls
- `SlidersHorizontal` - Filter controls
- `FileVideo` - Video file indicator
- `X` - Close/Remove actions
- `Users` - Peer count
- `Upload` (Icon) - Upload speed
- `Download` (Icon) - Download speed
- `Sun` / `Moon` / `Monitor` - Theme icons

---

## Responsive Breakpoints

- **Mobile**: < 640px (sm)
- **Tablet**: 640px - 768px (md)
- **Laptop**: 768px - 1024px (lg)
- **Desktop**: 1024px - 1280px (xl)
- **Wide**: > 1280px (2xl)

### Grid Columns by Device
- **Mobile**: 2 columns
- **Small Tablet**: 3 columns
- **Large Tablet**: 4 columns
- **Laptop**: 5 columns
- **Desktop+**: 6 columns

---

## Animation & Transitions

### Hover Effects
- Scale: 1.05 on movie cards
- Opacity: Controls fade in/out
- Background: Button hover states

### Transitions
- Duration: 200-300ms
- Easing: ease-in-out
- Properties: transform, opacity, background

### Loading States
- Pulse animation on skeletons
- Spin animation on loaders
- Fade in/out on page transitions

---

## Accessibility Features

✅ **Keyboard Navigation**
- Tab through all interactive elements
- Enter/Space to activate buttons
- Escape to close modals

✅ **Screen Reader Support**
- Semantic HTML
- ARIA labels
- Alt text on images

✅ **Focus Indicators**
- Visible focus rings
- High contrast outlines

✅ **Color Contrast**
- WCAG AA compliant
- Readable text on all backgrounds

---

## Best Practices Used

1. **TypeScript** - Full type safety
2. **Component Composition** - Reusable components
3. **Custom Hooks** - Logic abstraction
4. **Error Boundaries** - Graceful error handling
5. **Lazy Loading** - Performance optimization
6. **Responsive Design** - Mobile-first approach
7. **Accessibility** - WCAG guidelines
8. **Code Splitting** - Automatic with Next.js
9. **SEO Optimization** - Server-side rendering
10. **Security** - Protected routes, token refresh

---

This component showcase provides a complete overview of all UI elements and their features in the MovieHub frontend! 🎨
