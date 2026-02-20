# Google Play Submission Guide - PuzzleGame

## Prerequisites

Before you can submit to Google Play, you'll need:

1. **Google Play Developer Account**
   - Cost: $25 one-time registration
   - Visit: https://play.google.com/console
   - Time to setup: 5-10 minutes

2. **Signed APK** ✅
   - Location: `/root/.openclaw/workspace/PuzzleGameUnity/delivery/PuzzleGame-release.apk`
   - Configuration: Already signed with release keystore
   - Status: Ready for upload

3. **Google Play Store Listing Assets**
   - 2-4 Screenshots (see screenshots/ folder)
   - Feature graphic (1024x500px)
   - App icon (512x512px minimum)
   - Short description (80 characters max)
   - Full description (4000 characters max)
   - Promotional text (80 characters max)

---

## Step-by-Step Upload Instructions

### Phase 1: Google Play Console Setup (5 min)

1. Go to https://play.google.com/console
2. Sign in with your Google account
3. Click "Create app"
4. Fill in:
   - **App name**: "Puzzle Game"
   - **Default language**: English
   - **App or game**: Select "Game"
   - **Free or paid**: Free
   - **Content rating**: Follow the questionnaire
5. Click "Create app"

---

### Phase 2: Add App Details (15 min)

On the left sidebar, click **"App details"**:

1. **Organization**
   - Organization name: Your name
   - Organization address: Your address
   - Website URL: (optional)
   - Email: your-email@example.com
   - Phone: your-phone-number

2. **Store listing**
   - Title: "Puzzle Game"
   - Short description: "A fun and addictive puzzle game to challenge your mind."
   - Full description: (see GOOGLE_PLAY_SUBMISSION.md)
   - Promotional text: "New puzzles added monthly!"
   - App icon: Upload 512x512px PNG (see delivery/icons/)
   - Feature graphic: 1024x500px PNG (see delivery/graphics/)
   - Screenshots: Upload 4-8 screenshots (see delivery/screenshots/)

3. **Categories**
   - Select: "Puzzle"

4. **Content rating**
   - Complete questionnaire: Everyone/3+

---

### Phase 3: Upload APK Release (10 min)

On the left sidebar, click **"Testing"** → **"Internal testing"**:

1. Click "Create new release"
2. Click "Upload APK"
3. Browse and select: `PuzzleGame-release.apk`
4. Add release notes:
   ```
   Version 1.0.0 - Initial Launch
   
   Features:
   - 100+ unique puzzle levels
   - Smooth animations and effects
   - Challenging gameplay
   - Ad-supported with optional removal
   - Full offline gameplay
   ```
5. Click "Save"

---

### Phase 4: Review Before Publishing (10 min)

Click **"Release"** → **"Production"**:

1. Create new release with the same APK
2. Check all information is correct:
   - ✅ App icon
   - ✅ Screenshots
   - ✅ Descriptions
   - ✅ APK file
   - ✅ Content rating
3. Acknowledge:
   - ✅ "I confirm this app complies with Google Play Policies"
   - ✅ "I confirm this app is appropriate for the selected age category"
4. Click "Review release"

---

### Phase 5: Submit for Review (2 min)

1. Click "Start rollout to Production"
2. Select percentage: 100% (full release)
3. Click "Confirm rollout"
4. **Your app is now submitted!**

---

## Timeline

- **Submission**: Immediate (after you click submit)
- **Initial review**: 1-3 hours (usually)
- **Final status**: Check Google Play Console regularly
- **If approved**: Goes live in Play Store within 24 hours
- **If rejected**: You'll get detailed feedback to fix issues

---

## Troubleshooting

### "App not signed" error
- Ensure APK at delivery/ is the release build
- Contact support with error message

### "Content policy violation" warning
- Likely AdMob test ads issue
- Replace with real app ID in game config
- Contact us for help

### "Target API too low"
- Current: API 33 (Android 13)
- Google Play minimum: API 33 as of 2024
- You're compliant ✅

### "Insufficient content"
- Add 4+ distinct screenshots
- Add detailed feature description
- Add app icon and graphics

---

## After Launch

Once approved and live:

1. **Monitoring**
   - Check Google Play Console daily for first week
   - Monitor user feedback and ratings
   - Track crashes in Analytics

2. **Updates**
   - New version = new APK upload
   - Follow same process as initial launch
   - Subsequent reviews are usually 1-2 hours

3. **Support**
   - Set support email in app details
   - Respond to user reviews
   - Update description with version notes

---

## Important Files Location

```
/root/.openclaw/workspace/PuzzleGameUnity/
├── delivery/
│   ├── PuzzleGame-release.apk        ← Upload this!
│   ├── icons/
│   │   └── app-icon-512x512.png      ← App icon
│   ├── screenshots/
│   │   ├── screenshot-1.png
│   │   ├── screenshot-2.png
│   │   ├── screenshot-3.png
│   │   └── screenshot-4.png
│   └── graphics/
│       └── feature-graphic-1024x500.png
├── GOOGLE_PLAY_SUBMISSION_COPY_PASTE.md ← Copy text from here
└── BUILD_REPORT.md                  ← Reference for questions
```

---

## Quick Reference: What You Need

- ✅ APK file (signed and optimized)
- ✅ App icon (512x512 PNG)
- ✅ 2-4 screenshots (minimum 480x800px)
- ✅ Feature graphic (1024x500 PNG)
- ✅ Descriptions (short and long)
- ✅ Content rating completed
- ✅ Privacy policy (optional for simple games)

---

**Questions?** Check BUILD_REPORT.md or GOOGLE_PLAY_SUBMISSION.md for more details.

