# 🎬 MovieHub Frontend - Quick Start Guide

## ✅ What Has Been Built

I've created a **complete, production-ready movie streaming platform frontend** with all requested features!

## 📂 Important Note About File Structure

The complete application is in the **`src/`** directory. Next.js 14 supports both:
- `app/` directory (default create-next-app structure)  
- `src/app/` directory (cleaner organization)

**All the application code is in: `front-end/src/`**

## 🚀 Quick Start

### 1. Environment Setup
Create a `.env.local` file in the `front-end` directory:

```bash
cp .env.local.example .env.local
```

Edit `.env.local`:
```env
NEXT_PUBLIC_API_URL=http://localhost:8000/api
```

### 2. Install & Run
```bash
cd front-end
npm install
npm run dev
```

### 3. Open Your Browser
Navigate to: http://localhost:3000

## 📱 Available Pages

All pages are in `src/app/`:

| Route | Description |
|-------|-------------|
| `/` | Home page with trending, popular, top-rated movies |
| `/auth/login` | User login |
| `/auth/register` | User registration |
| `/auth/forgot-password` | Password recovery |
| `/browse` | Browse movies with search & filters |
| `/movie/[id]` | Movie detail page |
| `/watch/[id]` | Video player page |
| `/library` | User's library (My Movies, Continue Watching, Favorites) |
| `/upload` | Admin upload page (drag & drop) |
| `/settings` | User settings (Profile, Playback, Torrent, Appearance) |

## 🎯 Key Features Implemented

### ✅ Authentication
- Login / Register / Forgot Password
- JWT token management
- Auto token refresh
- Protected routes

### ✅ Movie Browsing
- Home page with hero section
- Trending, Popular, Top-Rated sections
- Advanced search with filters
- Genre filtering
- Responsive grid layout

### ✅ Movie Details
- Full movie information
- Cast members
- Related movies
- Add to library
- Favorite toggle
- Download support

### ✅ Video Player
- HLS streaming (hls.js)
- Custom controls
- Progress tracking
- Quality selector
- Subtitle support
- Fullscreen mode

### ✅ User Library
- My Movies
- Continue Watching
- Favorites
- Progress bars
- Watch history

### ✅ Admin Upload
- Drag & drop upload
- File validation
- Upload progress
- Metadata form
- Processing status

### ✅ Torrent Seeding
- WebTorrent integration
- Start/Stop seeding
- Real-time statistics
- Peer count
- Upload/Download speeds
- Seeding ratio

### ✅ Settings
- Profile information
- Playback settings
- Torrent preferences
- Theme selection

## 🛠️ Technology Stack

- **Framework**: Next.js 14 (App Router)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **UI Components**: ShadCN UI + Radix UI
- **State Management**: Redux Toolkit
- **Forms**: React Hook Form + Zod
- **HTTP Client**: Axios (with interceptors)
- **Video**: hls.js
- **Torrent**: WebTorrent
- **Icons**: Lucide React

## 📁 Project Structure

```
front-end/
├── src/                    ← ALL APPLICATION CODE HERE
│   ├── app/               # Pages & routes
│   ├── components/        # React components
│   ├── hooks/            # Custom hooks
│   ├── store/            # Redux store
│   ├── services/         # API services
│   ├── types/            # TypeScript types
│   └── lib/              # Utilities
├── app/                   # Default Next.js structure (can be ignored)
├── public/               # Static assets
├── .env.local           # Environment variables (create this)
└── package.json         # Dependencies

```

## 🔌 Backend API Integration

The frontend expects these microservices:

1. **Auth Service** - `/auth/*`
2. **Movie Service** - `/movies/*`
3. **Library Service** - `/library/*`
4. **Upload Service** - `/upload/*`
5. **Torrent Service** - `/torrent/*`

See `IMPLEMENTATION_SUMMARY.md` for complete API endpoint list.

## 🎨 Customization

### Change Theme Colors
Edit `src/app/globals.css` - CSS variables for colors

### Modify API URL
Edit `.env.local` - `NEXT_PUBLIC_API_URL`

### Add New Pages
Create in `src/app/[page-name]/page.tsx`

### Add New Components
Create in `src/components/[category]/[ComponentName].tsx`

## 🧪 Testing the Application

### Without Backend
The app will compile and run, showing UI components. API calls will fail gracefully.

### With Backend Running
1. Start your backend services
2. Update `.env.local` with correct API URL
3. Test each feature:
   - Authentication flow
   - Movie browsing
   - Video playback
   - Library management
   - Torrent seeding
   - File upload

## 📦 Build for Production

```bash
npm run build
npm start
```

Production build will be in `.next/` directory.

## 🔥 Hot Features

### 1. **HLS Video Streaming**
- File: `src/components/player/VideoPlayer.tsx`
- Uses hls.js for adaptive bitrate streaming
- Custom controls overlay
- Progress tracking with auto-save

### 2. **Torrent Seeding**
- File: `src/components/torrent/TorrentSeedWidget.tsx`
- WebTorrent for browser-based seeding
- Real-time stats updates every 5 seconds
- Peer connections tracking

### 3. **Redux State Management**
- Files: `src/store/slices/*.ts`
- Centralized state for: auth, movies, library, player, torrent
- Automatic localStorage sync for auth tokens

### 4. **Advanced Search**
- File: `src/components/movie/SearchBar.tsx`
- Filters: genre, year, quality, rating
- Sorting: date, rating, views, title

### 5. **Drag & Drop Upload**
- File: `src/app/upload/page.tsx`
- React Dropzone integration
- Real-time upload progress
- File type validation

## 🚨 Important Notes

1. **All your code is in `src/` directory** - that's where the full application lives
2. The `app/` directory at root is from create-next-app default structure
3. All dependencies are already installed
4. TypeScript is configured and ready
5. Tailwind CSS is configured with custom theme

## 📚 Documentation

- `README.md` - General overview
- `IMPLEMENTATION_SUMMARY.md` - Complete feature list & technical details
- `SETUP_GUIDE.md` - This file

## 🎉 You're Ready!

Everything is set up and ready to use. Just:
1. Configure your backend API URL
2. Run `npm run dev`
3. Start streaming movies! 🍿

## 💡 Tips

- Use Redux DevTools browser extension for state debugging
- Check browser console for API errors
- Test video player with sample HLS streams
- Verify torrent seeding in browser with test torrents

---

**Need help?** Check the implementation files in `src/` - they're well-commented and follow best practices!

Happy coding! 🚀
