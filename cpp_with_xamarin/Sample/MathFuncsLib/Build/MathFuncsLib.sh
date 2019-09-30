echo "========== Build All started =========="
echo ""

echo "Preparing working files and directories"

#Create directories and copy library files to working directory
{
    find . -name "bin" -type d -exec rm -rf "{}" \;
    find . -name "tmp" -type d -exec rm -rf "{}" \;
    mkdir bin
    mkdir bin/iOS
    mkdir bin/Android
    mkdir tmp    
    mkdir tmp/sourceFiles
    mkdir tmp/iOS
    mkdir tmp/Android

    find . -name "*.cpp" -exec cp {} tmp/sourceFiles \;
    find . -name "*.h" -exec cp {} tmp/sourceFiles \;
} &> /dev/null

declare -a AndroidArchitectures=("x86" "x86_64" "arm" "arm64")
declare -a iOSArchitectures=("x86_64" "arm64" "arm64e")

LibraryName="MathFuncs"
Android_NDK_Host_Name="macosx"
Android_NDK_Host_Tag="darwin-x86_64"
Android_Minimum_Api_Version="21"
iOS_SDK_Version="12.2"
iOS_SDK_Min_Version="8.0"

echo ""
echo "=== BUILD TARGET (Android) ==="
echo ""

cd tmp

LibPath=${PWD}/sourceFiles

if [ -z "${ANDROID_NDK_HOME}" ]
then
    export ANDROID_NDK_HOME="/Users/$USER/Library/Developer/Xamarin/android-sdk-$Android_NDK_Host_Name/ndk-bundle"
fi

for i in "${AndroidArchitectures[@]}"
    do
        echo "Build for $i:"

        ABI_Folder_Name=$i

        if [ $i == "arm" ]
        then
            ABI_Folder_Name="armeabi-v7a"
        elif [ $i == "arm64" ]
        then
            ABI_Folder_Name="arm64-v8a"
        fi

        mkdir ../bin/Android/$ABI_Folder_Name     

        if [ $i == "x86" ]
        then
            CxxTarget="i686-linux-android"
        elif [ $i == "x86_64" ]
        then
            CxxTarget="x86_64-linux-android"
        elif [ $i == "arm" ]
        then
            CxxTarget="armv7a-linux-androideabi"
        else
            CxxTarget="aarch64-linux-android"
        fi

        CxxTarget="$ANDROID_NDK_HOME/toolchains/llvm/prebuilt/$Android_NDK_Host_Tag/bin/$CxxTarget$Android_Minimum_Api_Version-clang++"   

        export CXX=$CxxTarget

        echo "Compiling and linking (output as dynamic library)"

        for i2 in $LibPath/*.cpp; do
            ShortName="${i2##*/}"
            OutputName="${ShortName/cpp/o}"
            $CXX -c $i2 -std=c++0x -fPIC -o ${PWD}/sourceFiles/$OutputName
        done

        $CXX -shared -static-libstdc++ -o ${PWD}/sourceFiles/lib${LibraryName}.so ${PWD}/sourceFiles/*.o

        echo "Copying lib${LibraryName}.so to bin/Android/$ABI_Folder_Name"
        {
            find sourceFiles -name "*.so" -exec cp {} ../bin/Android/$ABI_Folder_Name \;
        } &> /dev/null

        echo ""

    done

cd ..
echo "** BUILD SUCCEEDED (Android) **"
echo ""     

echo ""
echo "=== BUILD TARGET (iOS) ==="
echo ""

cd tmp

for i in "${iOSArchitectures[@]}"
do
    SdkRootValue="iPhoneOS"
    echo "Build for $i:"
    if [ $i == "x86_64" ]
    then
        SdkRootValue="iPhoneSimulator"
    fi

    export DEVROOT=/Applications/Xcode.app/Contents/Developer/Platforms/$SdkRootValue.platform/Developer
    export IPHONEOS_DEPLOYMENT_TARGET=$iOS_SDK_Version
    export SDKROOT=$DEVROOT/SDKs/$SdkRootValue.sdk
    export CFLAGS="-std=c++0x -arch $i -pipe -no-cpp-precomp -fembed-bitcode -isysroot $SDKROOT -miphoneos-version-min=$iOS_SDK_Min_Version -I$SDKROOT/usr/include/"

    echo "Compiling and linking (output as static library)"

    cd sourceFiles
    g++ -c *.cpp $CFLAGS
    cd ..

    {
        ar ru iOS/${LibraryName}_${i}.a sourceFiles/*.o
    } &> /dev/null    

    cd sourceFiles
    find . -name "*.o" -type f -delete
    cd ..

    echo ""
done

echo "Build universal library:"
lipo iOS/*.a -output iOS/lib$LibraryName.a -create

echo "Copying lib${LibraryName}.a to bin/iOS"
{
    find iOS -name "lib${LibraryName}.a" -exec cp {} ../bin/iOS \;
} &> /dev/null

cd ..

echo ""
echo "** BUILD SUCCEEDED (iOS) **"
echo "" 

echo "========== Build All completed =========="
echo ""

cd ..

#Cleanup working directories
{
    find . -name "tmp" -type d -exec rm -rf "{}" \;
} &> /dev/null