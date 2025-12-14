# MovieHub Frontend

Modern, responsive Next.js application for the MovieHub streaming platform.

## 🚀 Quick Start

```bash
# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Start production server
npm start
```

**Access:** http://localhost:3000

## 📚 Full Documentation

See **[FRONTEND.md](../FRONTEND.md)** for complete documentation including:
- Full architecture overview
- Component structure
- State management
- API integration
- HLS video player setup
- WebTorrent integration
- Development guide

## 🛠️ Tech Stack

- Next.js 16 (React 19)
- TypeScript
- Tailwind CSS
- Redux Toolkit
- HLS.js & WebTorrent

## 📡 Environment Variables

Create `.env.local`:
```env
NEXT_PUBLIC_API_URL=http://localhost:5001/api/v1
```

## 🐳 Docker

```bash
# Build
docker build -t moviehub-frontend .

# Run
docker run -p 3000:3000 moviehub-frontend
```

---

**For detailed documentation, see [FRONTEND.md](../FRONTEND.md)**
