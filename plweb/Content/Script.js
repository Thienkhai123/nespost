(function() {
    var adjustmentDelegates = [];
    function AddAdjustmentDelegate(adjustmentDelegate) {
        adjustmentDelegates.push(adjustmentDelegate);
    }
    function onControlsInitialized(s, e) {
        adjustPageControls();
    }
    function onBrowserWindowResized(s, e) {
        adjustPageControls();
    }
    function adjustPageControls() {
        for(var i = 0; i < adjustmentDelegates.length; i++) {
            adjustmentDelegates[i]();
        }
    }

    function validateEmail(email) {
        if (email == "")
            return true;
        const re = /^[a-z][a-z0-9_\.]{5,32}@[a-z0-9]{2,}(\.[a-z0-9]{2,4}){1,2}$/;
        return re.test(email);
    }

    window.onControlsInitialized = onControlsInitialized;
    window.onBrowserWindowResized = onBrowserWindowResized;
    window.adjustPageControls = adjustPageControls;
    window.AddAdjustmentDelegate = AddAdjustmentDelegate;
    window.validateEmail = validateEmail;
})();