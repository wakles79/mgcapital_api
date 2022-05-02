import { MobilePlatform, MobileApp } from '@app/core/models/mobile/mobile-app.enum';

export interface MobileAppVersion {
  platform: MobilePlatform;
  mobileApp: MobileApp;
  versionNumber: string;
  url: string;
  lastAvailableVersion: {
    major: number,
    minor: number,
    build: number,
    revision: number,
    majorRevision: number,
    minorRevision: number,
  };
}
