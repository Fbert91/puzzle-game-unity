# APK Testing & Verification Framework
# PuzzleGameUnity - Build Verification Tests

echo "=== APK Build Verification Framework ===" > test_results.txt
echo "Date: $(date)" >> test_results.txt
echo "Project: PuzzleGameUnity" >> test_results.txt
echo "" >> test_results.txt

# Function to check APK exists and is valid
check_apk_exists() {
    local apk_file="$1"
    if [ -f "$apk_file" ]; then
        echo "✅ APK file exists: $apk_file" | tee -a test_results.txt
        return 0
    else
        echo "❌ APK file not found: $apk_file" | tee -a test_results.txt
        return 1
    fi
}

# Function to check APK file size
check_apk_size() {
    local apk_file="$1"
    local size=$(du -b "$apk_file" | cut -f1)
    local size_mb=$((size / 1024 / 1024))
    
    if [ $size_mb -gt 50 ] && [ $size_mb -lt 500 ]; then
        echo "✅ APK size reasonable: ${size_mb}MB (50-500MB acceptable)" | tee -a test_results.txt
        return 0
    else
        echo "⚠️  APK size outside normal range: ${size_mb}MB" | tee -a test_results.txt
        return 1
    fi
}

# Function to verify APK is valid ZIP
check_apk_valid() {
    local apk_file="$1"
    if unzip -t "$apk_file" &>/dev/null; then
        echo "✅ APK is valid ZIP archive" | tee -a test_results.txt
        return 0
    else
        echo "❌ APK is not a valid ZIP file" | tee -a test_results.txt
        return 1
    fi
}

# Function to check AndroidManifest
check_manifest() {
    local apk_file="$1"
    if unzip -l "$apk_file" | grep -q "AndroidManifest.xml"; then
        echo "✅ AndroidManifest.xml present" | tee -a test_results.txt
        # Extract and check content
        unzip -p "$apk_file" AndroidManifest.xml > /tmp/manifest.xml 2>/dev/null
        if grep -q "com.fbert91.puzzlegame" /tmp/manifest.xml; then
            echo "✅ Bundle identifier correct: com.fbert91.puzzlegame" | tee -a test_results.txt
            return 0
        fi
    fi
    echo "❌ AndroidManifest.xml missing or invalid" | tee -a test_results.txt
    return 1
}

# Function to check resources
check_resources() {
    local apk_file="$1"
    if unzip -l "$apk_file" | grep -q "res/"; then
        echo "✅ Resources directory present" | tee -a test_results.txt
        # Count resource files
        local res_count=$(unzip -l "$apk_file" | grep "res/" | wc -l)
        echo "   - Resource files: $res_count" | tee -a test_results.txt
        return 0
    else
        echo "❌ No resources found in APK" | tee -a test_results.txt
        return 1
    fi
}

# Function to check classes.dex
check_dex() {
    local apk_file="$1"
    if unzip -l "$apk_file" | grep -q "classes.dex"; then
        echo "✅ classes.dex present" | tee -a test_results.txt
        # Count total dex files
        local dex_count=$(unzip -l "$apk_file" | grep "classes.*\.dex" | wc -l)
        echo "   - DEX files: $dex_count" | tee -a test_results.txt
        return 0
    else
        echo "❌ No DEX files found in APK" | tee -a test_results.txt
        return 1
    fi
}

# Function to check native libraries
check_native_libs() {
    local apk_file="$1"
    if unzip -l "$apk_file" | grep -q "lib/arm"; then
        echo "✅ Native libraries (ARM) present" | tee -a test_results.txt
        # List architectures
        local archs=$(unzip -l "$apk_file" | grep "lib/" | grep -oE "lib/[^/]+" | sort -u)
        echo "   - Architectures: $archs" | tee -a test_results.txt
        return 0
    else
        echo "⚠️  No native libraries found (may not be needed)" | tee -a test_results.txt
        return 0
    fi
}

# Function to check assets
check_assets() {
    local apk_file="$1"
    if unzip -l "$apk_file" | grep -q "assets/"; then
        echo "✅ Game assets bundled" | tee -a test_results.txt
        local assets_count=$(unzip -l "$apk_file" | grep "assets/" | wc -l)
        echo "   - Asset files: $assets_count" | tee -a test_results.txt
        return 0
    else
        echo "⚠️  No assets directory found" | tee -a test_results.txt
        return 0
    fi
}

echo "Run check_apk_exists <apk_path>" > /tmp/test_functions.txt
echo "Run check_apk_size <apk_path>" >> /tmp/test_functions.txt
echo "All test functions defined" 
