pushd $UNITY_DIR/Builds/$BUILD_TARGET/ > /dev/null
chmod +x $BUILD_NAME
echo "Running server!"
./$BUILD_NAME
echo "Exited server!"
popd > /dev/null
