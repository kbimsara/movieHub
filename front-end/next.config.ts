import type { NextConfig } from 'next';

const nextConfig: NextConfig = {
  output: 'standalone',
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: '**',
      },
      {
        // Local dev – API gateway on localhost
        protocol: 'http',
        hostname: 'localhost',
        port: '5000',
      },
      {
        // Docker internal – API gateway container hostname
        protocol: 'http',
        hostname: 'api-gateway',
      },
      {
        // Loopback fallback
        protocol: 'http',
        hostname: '127.0.0.1',
      },
    ],
  },
  // Next.js 16 uses Turbopack by default. This empty config acknowledges
  // that the webpack config below is intentional (for legacy/non-Turbopack builds)
  // and silences the "webpack config with no turbopack config" build error.
  turbopack: {},
  webpack: (config, { isServer }) => {
    // WebTorrent needs these for browser environment
    if (!isServer) {
      config.resolve.fallback = {
        ...config.resolve.fallback,
        fs: false,
        net: false,
        tls: false,
      };
    }
    return config;
  },
};

export default nextConfig;

