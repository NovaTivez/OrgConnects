app.controller("OrgConnectController", function ($scope, OrgConnectService) {

    $scope.userarray = [];
    $scope.isEditing = false;
    $scope.editingIndex = -1;

    // Load data

    $scope.loadRegistrationDataFunc = function () {
        $scope.optionsLoading = true;
        $scope.committeeList = [];
        $scope.positionList = [];
        $scope.registrationOptions = [];

        var getData = OrgConnectService.GetRegistrationDataFunc();
        getData.then(function (returnedData) {
            $scope.optionsLoading = false;
            if (!angular.isArray(returnedData.data)) {
                $scope.SweetAlertError(returnedData.data && returnedData.data.Message);
                return;
            }
            $scope.registrationOptions = returnedData.data;
            angular.forEach($scope.registrationOptions, function (item) {
                if ($scope.committeeList.indexOf(item.Committee) === -1) {
                    $scope.committeeList.push(item.Committee);
                }
            });
        }, function () {
            $scope.optionsLoading = false;
            $scope.SweetAlertError("Could not load committees and roles.");
        });
    };

    $scope.loadMembersFunc = function () {
        if ($scope.isSaving || $scope.isDeleting) {
            return;
        }
        var getData = OrgConnectService.GetMembersFunc();
        getData.then(function (returnedData) {
            if (!angular.isArray(returnedData.data)) {
                $scope.SweetAlertError(returnedData.data && returnedData.data.Message);
                return;
            }
            $scope.userarray = returnedData.data;
            $scope.clearRegistrationFunc();
        }, function () {
            $scope.SweetAlertError("Could not load members.");
        });
    };

    $scope.emailPattern = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$/;

    $scope.passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/;

    $scope.validatePasswordMatch = function (registrationForm) {
        if (!registrationForm || !registrationForm.confirmPassword) {
            return;
        }

        var passwordsMatch = !$scope.confirmPassword || $scope.password === $scope.confirmPassword;
        registrationForm.confirmPassword.$setValidity("passwordMatch", passwordsMatch);
    };

    // Validation

    $scope.isDuplicateMemberID = function () {
        var enteredID = ($scope.memberID || "").toLowerCase();

        return $scope.userarray.some(function (user, index) {
            return index !== $scope.editingIndex && user.MemberID.toLowerCase() === enteredID;
        });
    };

    $scope.isDuplicateUsername = function () {
        var enteredUsername = ($scope.username || "").toLowerCase();

        return $scope.userarray.some(function (user, index) {
            return index !== $scope.editingIndex && (user.Username || "").toLowerCase() === enteredUsername;
        });
    };

    $scope.isDuplicateEmail = function () {
        var enteredEmail = ($scope.email || "").trim().toLowerCase();

        return $scope.userarray.some(function (user, index) {
            return index !== $scope.editingIndex && (user.Email || "").trim().toLowerCase() === enteredEmail;
        });
    };

    $scope.isDuplicateContactNumber = function () {
        var enteredNumber = $scope.contactNumber || "";

        return $scope.userarray.some(function (user, index) {
            return index !== $scope.editingIndex && user.ContactNumber === enteredNumber;
        });
    };

    // Contact number
    $scope.numbersOnlyFunc = function () {
        $scope.contactNumber = ($scope.contactNumber || "")
            .replace(/[^0-9]/g, "")
            .slice(0, 11);
    };

    $scope.committeeChangedFunc = function () {
        $scope.position = "";
        $scope.positionList = [];
        angular.forEach($scope.registrationOptions, function (item) {
            if (item.Committee === $scope.committee && $scope.positionList.indexOf(item.Position) === -1) {
                $scope.positionList.push(item.Position);
            }
        });
    };

    // Save member

    $scope.formIsValid = function (registrationForm) {
        return !registrationForm.$invalid &&
            !$scope.isDuplicateMemberID() &&
            !$scope.isDuplicateUsername() &&
            !$scope.isDuplicateEmail() &&
            !$scope.isDuplicateContactNumber();
    };

    $scope.registrationFunc = function (registrationForm) {
        $scope.registrationSubmitted = true;
        // Registration validation messages
        if (registrationForm.firstName.$error.required) {
            $scope.SweetAlertError("First name is required.");
            return;
        }

        if (registrationForm.firstName.$error.minlength || registrationForm.firstName.$error.maxlength) {
            $scope.SweetAlertError("First name must contain 2–50 characters.");
            return;
        }

        if (registrationForm.firstName.$error.pattern) {
            $scope.SweetAlertError("Use letters, spaces, apostrophes, periods, and hyphens only.");
            return;
        }

        if (registrationForm.middleName.$error.pattern || registrationForm.middleName.$error.maxlength) {
            $scope.SweetAlertError("Middle name may contain up to 50 characters: letters, spaces, apostrophes, periods, and hyphens only.");
            return;
        }

        if (registrationForm.lastName.$error.required) {
            $scope.SweetAlertError("Last name is required.");
            return;
        }

        if (registrationForm.lastName.$error.minlength || registrationForm.lastName.$error.maxlength) {
            $scope.SweetAlertError("Last name must contain 2–50 characters.");
            return;
        }

        if (registrationForm.lastName.$error.pattern) {
            $scope.SweetAlertError("Use letters, spaces, apostrophes, periods, and hyphens only.");
            return;
        }

        if (registrationForm.memberID.$error.required) {
            $scope.SweetAlertError("Member ID is required.");
            return;
        }

        if (registrationForm.memberID.$error.pattern) {
            $scope.SweetAlertError("Use the format ORG-0001.");
            return;
        }

        if ($scope.memberID && registrationForm.memberID.$valid && $scope.isDuplicateMemberID()) {
            $scope.SweetAlertError("Member ID already exists.");
            return;
        }

        if (registrationForm.username.$error.required) {
            $scope.SweetAlertError("Username is required.");
            return;
        }

        if (registrationForm.username.$error.minlength || registrationForm.username.$error.maxlength) {
            $scope.SweetAlertError("Username must contain 4–20 characters.");
            return;
        }

        if (registrationForm.username.$error.pattern) {
            $scope.SweetAlertError("Use letters, numbers, periods, underscores, or hyphens only.");
            return;
        }

        if ($scope.username && registrationForm.username.$valid && $scope.isDuplicateUsername()) {
            $scope.SweetAlertError("Username already exists.");
            return;
        }

        if (registrationForm.email.$error.required) {
            $scope.SweetAlertError("Email address is required.");
            return;
        }

        if (!registrationForm.email.$error.required && (registrationForm.email.$error.email || registrationForm.email.$error.pattern)) {
            $scope.SweetAlertError("Enter a complete email address, such as name@example.com.");
            return;
        }

        if ($scope.email && registrationForm.email.$valid && $scope.isDuplicateEmail()) {
            $scope.SweetAlertError("Email address already exists.");
            return;
        }

        if (registrationForm.contactNumber.$error.required) {
            $scope.SweetAlertError("Contact number is required.");
            return;
        }

        if (registrationForm.contactNumber.$error.pattern) {
            $scope.SweetAlertError("Enter an 11-digit Philippine mobile number.");
            return;
        }

        if ($scope.contactNumber && registrationForm.contactNumber.$valid && $scope.isDuplicateContactNumber()) {
            $scope.SweetAlertError("Contact number already exists.");
            return;
        }

        if (registrationForm.password.$error.required) {
            $scope.SweetAlertError("Password is required.");
            return;
        }

        if (!registrationForm.password.$error.required && (registrationForm.password.$error.minlength || registrationForm.password.$error.maxlength)) {
            $scope.SweetAlertError("Password must contain 8–32 characters.");
            return;
        }

        if (!registrationForm.password.$error.required && !registrationForm.password.$error.minlength && !registrationForm.password.$error.maxlength && registrationForm.password.$error.pattern) {
            $scope.SweetAlertError("Include uppercase, lowercase, number, and special character.");
            return;
        }

        if (registrationForm.confirmPassword.$error.required) {
            $scope.SweetAlertError("Please confirm the password.");
            return;
        }

        if (!registrationForm.confirmPassword.$error.required && registrationForm.confirmPassword.$error.passwordMatch) {
            $scope.SweetAlertError("Passwords do not match.");
            return;
        }

        if (registrationForm.committee.$error.required) {
            $scope.SweetAlertError("Group or committee is required.");
            return;
        }

        if (registrationForm.position.$error.required) {
            $scope.SweetAlertError("Role is required.");
            return;
        }

        if (!$scope.formIsValid(registrationForm)) {
            $scope.SweetAlertError("Please check the registration details.");
            return;
        }

        if ($scope.isSaving || $scope.isDeleting) {
            return;
        }
        var isUpdating = $scope.isEditing;
        if (isUpdating && !$scope.userarray[$scope.editingIndex]) {
            return;
        }

        $scope.isSaving = true;
        var userData = {
            MemberID: $scope.memberID.toUpperCase(),
            FName: $scope.firstName,
            MName: ($scope.middleName || "").trim(),
            LName: $scope.lastName,
            Username: $scope.username,
            Email: $scope.email.trim().toLowerCase(),
            ContactNumber: $scope.contactNumber,
            Password: $scope.password,
            ConfirmPassword: $scope.confirmPassword,
            Committee: $scope.committee,
            Position: $scope.position
        };

        var postData;
        if (isUpdating) {
            userData.DatabaseID = $scope.userarray[$scope.editingIndex].DatabaseID;
            postData = OrgConnectService.UpdateMemberFunc(userData);
        } else {
            postData = OrgConnectService.CreateMemberFunc(userData);
        }

        postData.then(function (returnedData) {
            $scope.isSaving = false;
            if (!angular.isArray(returnedData.data)) {
                $scope.SweetAlertError(returnedData.data && returnedData.data.Message);
                return;
            }
            $scope.userarray = returnedData.data;
            $scope.clearRegistrationFunc(registrationForm);
            $scope.SweetAlertSuccess(isUpdating ? "Member updated." : "Member registered.");
        }, function () {
            $scope.isSaving = false;
            $scope.SweetAlertError("Could not save the member. Please try again.");
        });
    };

    // Edit member
    $scope.editFunc = function (userindex, registrationForm) {
        if ($scope.isSaving || $scope.isDeleting || !$scope.userarray[userindex]) {
            return;
        }
        var user = $scope.userarray[userindex];
        $scope.isEditing = true;
        $scope.editingIndex = userindex;
        $scope.registrationSubmitted = false;

        $scope.memberID = user.MemberID;
        $scope.firstName = user.FName;
        $scope.middleName = user.MName || "";
        $scope.lastName = user.LName;
        $scope.username = user.Username;
        $scope.email = user.Email;
        $scope.contactNumber = user.ContactNumber;
        $scope.password = "";
        $scope.confirmPassword = "";
        $scope.committee = user.Committee;
        $scope.committeeChangedFunc();
        $scope.position = $scope.positionList.indexOf(user.Position) >= 0 ? user.Position : "";
        $scope.validatePasswordMatch(registrationForm);
        if (registrationForm) {
            registrationForm.$setPristine();
            registrationForm.$setUntouched();
        }
        setTimeout(function () {
            if (window.M && window.M.updateTextFields) {
                window.M.updateTextFields();
            }
        }, 0);
    };

    $scope.deleteFunc = function (userindex) {
        if ($scope.isSaving || $scope.isDeleting) {
            return;
        }
        var user = $scope.userarray[userindex];
        if (!user || !window.confirm("Delete " + user.FName + " " + user.LName + " and their account?")) {
            return;
        }

        $scope.isDeleting = true;
        OrgConnectService.RemoveMemberFunc(user.DatabaseID).then(function (returnedData) {
            $scope.isDeleting = false;
            if (!angular.isArray(returnedData.data)) {
                $scope.SweetAlertError(returnedData.data && returnedData.data.Message);
                return;
            }
            $scope.userarray = returnedData.data;
            $scope.clearRegistrationFunc();
            $scope.SweetAlertSuccess("Member deleted.");
        }, function () {
            $scope.isDeleting = false;
            $scope.SweetAlertError("Could not delete the member.");
        });
    };

    // Clear form

    $scope.clearRegistrationFunc = function (registrationForm) {
        $scope.isEditing = false;
        $scope.editingIndex = -1;

        $scope.memberID = "";
        $scope.firstName = "";
        $scope.middleName = "";
        $scope.lastName = "";
        $scope.username = "";
        $scope.email = "";
        $scope.contactNumber = "";
        $scope.password = "";
        $scope.confirmPassword = "";
        $scope.committee = "";
        $scope.position = "";
        $scope.positionList = [];

        $scope.registrationSubmitted = false;

        if (registrationForm) {
            $scope.validatePasswordMatch(registrationForm);
            registrationForm.$setPristine();
            registrationForm.$setUntouched();
        }

        setTimeout(function () {
            if (window.M && window.M.updateTextFields) {
                window.M.updateTextFields();
            }
        }, 0);
    };

    // Login

    $scope.loginFunc = function (loginForm) {
        $scope.loginSubmitted = true;

        if (loginForm.$invalid) {
            $scope.SweetAlertError("Please enter a valid username and password.");
            return;
        }

        Swal.fire({ title: "Login Not Connected Yet", text: "This MP3 version saves registration records. Account login will be implemented in a later stage.", icon: "info", confirmButtonColor: "#0a4caf" });
    };

    $scope.clearLoginFunc = function (loginForm) {
        $scope.loginUsername = "";
        $scope.loginPassword = "";
        $scope.loginSubmitted = false;

        if (loginForm) {
            loginForm.$setPristine();
            loginForm.$setUntouched();
        }
    };

    $scope.registrationRedirectFunc = function () {
        window.location.href = "/Main/RegistrationPage";
    };

    $scope.SweetAlertError = function (message) {
        Swal.fire({
            title: "Error!",
            text: message || "The request could not be completed.",
            icon: "error",
            confirmButtonColor: "#0a4caf"
        });
    };

    $scope.SweetAlertSuccess = function (message) {
        Swal.fire({
            title: "Success!",
            text: message,
            icon: "success",
            confirmButtonColor: "#0a4caf"
        });
    };

});
