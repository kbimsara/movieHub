'use client';

import { useEffect, useCallback } from 'react';
import { useAuth } from '@/hooks/useAuth';
import { isSessionValid, getTimeUntilExpiry, extendSession } from '@/utils/session';

/**
 * SessionMonitor Component
 * Monitors user session and handles automatic logout on session expiry
 * Also extends session on user activity
 */
export function SessionMonitor() {
  const { logout, isAuthenticated } = useAuth();

  // Check session validity periodically
  const checkSession = useCallback(() => {
    if (!isAuthenticated) return;

    if (!isSessionValid()) {
      console.warn('⚠️ Session expired, logging out...');
      logout();
      return;
    }

    // Log time until expiry for debugging
    const timeLeft = getTimeUntilExpiry();
    if (timeLeft !== null) {
      const minutes = Math.floor(timeLeft / (60 * 1000));
      if (minutes <= 30) {
        console.log(`⏰ Session expires in ${minutes} minutes`);
      }
    }
  }, [isAuthenticated, logout]);

  // Extend session on user activity
  const handleUserActivity = useCallback(() => {
    if (!isAuthenticated) return;
    
    // Extend session on activity
    extendSession();
  }, [isAuthenticated]);

  useEffect(() => {
    if (!isAuthenticated) return;

    // Check session every minute
    const sessionCheckInterval = setInterval(checkSession, 60 * 1000);

    // Activity events to monitor
    const activityEvents = ['mousedown', 'keydown', 'scroll', 'touchstart'];
    
    // Throttle activity handler to prevent too many calls
    let activityTimeout: NodeJS.Timeout;
    const throttledActivityHandler = () => {
      clearTimeout(activityTimeout);
      activityTimeout = setTimeout(handleUserActivity, 5 * 60 * 1000); // Extend every 5 minutes of activity
    };

    // Add activity listeners
    activityEvents.forEach(event => {
      window.addEventListener(event, throttledActivityHandler);
    });

    // Initial check
    checkSession();

    // Cleanup
    return () => {
      clearInterval(sessionCheckInterval);
      clearTimeout(activityTimeout);
      activityEvents.forEach(event => {
        window.removeEventListener(event, throttledActivityHandler);
      });
    };
  }, [isAuthenticated, checkSession, handleUserActivity]);

  // This component doesn't render anything
  return null;
}
