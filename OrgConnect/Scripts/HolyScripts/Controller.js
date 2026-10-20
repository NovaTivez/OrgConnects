app.controller("OrgConnectController",
    function ($scope, OrgConnectService) {

        // login

        $scope.loginSubmitted = false;

        $scope.loginFunc = function (loginForm) {
            $scope.loginSubmitted = true;

            if (loginForm.$invalid) {
                Swal.fire({ title: "Login Failed", text: "Please enter a valid username and password.", icon: "error", confirmButtonColor: "#0a4caf" });
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


        // registration data

        $scope.userarray = [];
        $scope.isEditing = false;
        $scope.editingIndex = -1;
        $scope.isDeleting = false;
        $scope.committeeList = [];
        $scope.positionList = [];
        $scope.registrationOptions = [];
        $scope.isSaving = false;
        $scope.optionsLoading = false;
        $scope.optionsError = "";
        $scope.loadRegistrationDataFunc = function () {
            $scope.optionsLoading = true;
            $scope.optionsError = "";
            $scope.committeeList = [];
            $scope.positionList = [];
            $scope.registrationOptions = [];
            OrgConnectService.GetRegistrationDataFunc().then(function (response) {
                $scope.optionsLoading = false;
                if (!response.data.Success) {
                    $scope.optionsError = response.data.Message;
                    return;
                }
                $scope.registrationOptions = response.data.Options;
                angular.forEach($scope.registrationOptions, function (item) {
                    if ($scope.committeeList.indexOf(item.Committee) === -1) {
                        $scope.committeeList.push(item.Committee);
                    }
                });
                if (!$scope.committeeList.length) {
                    $scope.optionsError = "No available committee-position assignments. Run the initial-data SQL first.";
                }
            }, function () {
                $scope.optionsLoading = false;
                $scope.optionsError = "Could not load committees and positions. Check the database connection.";
            });
        };

        $scope.membersLoading = false;
        $scope.membersError = "";
        $scope.loadMembersFunc = function () {
            if ($scope.isSaving || $scope.isDeleting || $scope.membersLoading) { return; }
            $scope.membersLoading = true;
            $scope.membersError = "";
            var getData = OrgConnectService.GetMembersFunc();
            getData.then(function (returnedData) {
                $scope.membersLoading = false;
                if (!returnedData.data.Success) {
                    $scope.membersError = returnedData.data.Message;
                    return;
                }
                $scope.userarray = returnedData.data.Members;
                $scope.clearRegistrationFunc();
            }, function () {
                $scope.membersLoading = false;
                $scope.membersError = "Could not load saved members. Check the database connection and retry.";
            });
        };

        $scope.registrationSubmitted = false;
        $scope.serverErrors = {};


        $scope.emailPattern = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$/;

        $scope.passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/;

        $scope.validatePasswordMatch = function (registrationForm) {
            if (!registrationForm || !registrationForm.confirmPassword) {
                return;
            }

            var passwordsMatch = !$scope.confirmPassword ||
                $scope.password === $scope.confirmPassword;

            registrationForm.confirmPassword.$setValidity(
                "passwordMatch",
                passwordsMatch
            );
        };


        // duplicate checks

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

        // removes letters and symbols from the contact number
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

        // create

        $scope.registrationFunc = function (registrationForm) {
            $scope.registrationSubmitted = true;
            $scope.serverErrors = {};

            if (registrationForm.$invalid) {
                var labels = { memberID: "Member ID", username: "Username", firstName: "First Name", middleName: "Middle Name", lastName: "Last Name", email: "Email", contactNumber: "Contact Number", password: "Password", confirmPassword: "Confirm Password", committee: "Group / Committee", position: "Role" };
                var invalidFields = [];
                angular.forEach(registrationForm.$error, function (controls) {
                    angular.forEach(controls, function (control) {
                        if (control.$setTouched) { control.$setTouched(); }
                        var label = labels[control.$name] || control.$name;
                        if (label && invalidFields.indexOf(label) === -1) { invalidFields.push(label); }
                    });
                });
                Swal.fire({ title: "Invalid Information", text: "Please correct the highlighted fields." + (invalidFields.length ? "\nFields: " + invalidFields.join(", ") : ""), icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }


            if ($scope.isDuplicateMemberID()) {
                Swal.fire({ title: "Duplicate Member ID", text: "The entered Member ID already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateUsername()) {
                Swal.fire({ title: "Duplicate Username", text: "The entered username already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateEmail()) {
                Swal.fire({ title: "Duplicate Email", text: "The entered email address already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateContactNumber()) {
                Swal.fire({ title: "Duplicate Contact Number", text: "The entered contact number already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isSaving || $scope.isDeleting) {
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
            OrgConnectService.CreateMemberFunc(userData).then(function (returnedData) {
                $scope.isSaving = false;
                var result = returnedData.data;
                if (!result.Success) {
                    $scope.serverErrors = result.FieldErrors || {};
                    Swal.fire({ title: result.Title || "Registration Failed", text: result.Message, icon: "error", confirmButtonColor: "#0a4caf" });
                    return;
                }
                $scope.userarray = result.Members;
                Swal.fire({ title: "Registration Successful", text: "The organization member was registered.", icon: "success", confirmButtonColor: "#0a4caf" });
                $scope.clearRegistrationFunc(registrationForm);
            }, function () {
                $scope.isSaving = false;
                Swal.fire({ title: "Registration Failed", text: "The request failed. Refresh the page and check the database connection before trying again.", icon: "error", confirmButtonColor: "#0a4caf" });
            });
        };

        // Edit the form, then submit changes through the service and C# controller.
        $scope.editFunc = function (userindex, registrationForm) {
            if ($scope.isSaving || $scope.isDeleting || !$scope.userarray[userindex]) {
                return;
            }
            var user = $scope.userarray[userindex];
            $scope.isEditing = true;
            $scope.editingIndex = userindex;
            $scope.registrationSubmitted = false;
            $scope.serverErrors = {};
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
            window.scrollTo({ top: 0, behavior: "smooth" });
        };

        $scope.updateFunc = function (registrationForm) {
            $scope.registrationSubmitted = true;
            $scope.serverErrors = {};

            if (registrationForm.$invalid) {
                var labels = { memberID: "Member ID", username: "Username", firstName: "First Name", middleName: "Middle Name", lastName: "Last Name", email: "Email", contactNumber: "Contact Number", password: "Password", confirmPassword: "Confirm Password", committee: "Group / Committee", position: "Role" };
                var invalidFields = [];
                angular.forEach(registrationForm.$error, function (controls) {
                    angular.forEach(controls, function (control) {
                        if (control.$setTouched) { control.$setTouched(); }
                        var label = labels[control.$name] || control.$name;
                        if (label && invalidFields.indexOf(label) === -1) { invalidFields.push(label); }
                    });
                });
                Swal.fire({ title: "Invalid Information", text: "Please correct the highlighted fields." + (invalidFields.length ? "\nFields: " + invalidFields.join(", ") : ""), icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }


            if ($scope.isDuplicateMemberID()) {
                Swal.fire({ title: "Duplicate Member ID", text: "The entered Member ID already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateUsername()) {
                Swal.fire({ title: "Duplicate Username", text: "The entered username already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateEmail()) {
                Swal.fire({ title: "Duplicate Email", text: "The entered email address already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isDuplicateContactNumber()) {
                Swal.fire({ title: "Duplicate Contact Number", text: "The entered contact number already exists.", icon: "error", confirmButtonColor: "#0a4caf" });
                return;
            }

            if ($scope.isSaving || $scope.isDeleting || !$scope.isEditing || !$scope.userarray[$scope.editingIndex]) {
                return;
            }
            var originalRow = $scope.userarray[$scope.editingIndex];
            $scope.isSaving = true;
            var userData = {
                DatabaseID: originalRow.DatabaseID,
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
            OrgConnectService.UpdateMemberFunc(userData).then(function (returnedData) {
                $scope.isSaving = false;
                var result = returnedData.data;
                if (!result.Success) {
                    $scope.serverErrors = result.FieldErrors || {};
                    Swal.fire({ title: result.Title || "Update Failed", text: result.Message, icon: "error", confirmButtonColor: "#0a4caf" });
                    return;
                }
                $scope.userarray = result.Members;
                $scope.clearRegistrationFunc(registrationForm);
                Swal.fire({ title: "Update Successful", text: "The member information was updated in the database.", icon: "success", confirmButtonColor: "#0a4caf" });
            }, function () {
                $scope.isSaving = false;
                Swal.fire({ title: "Update Failed", text: "The request failed. Check the database connection before trying again.", icon: "error", confirmButtonColor: "#0a4caf" });
            });
        };

        $scope.deleteFunc = function (userindex) {
            if ($scope.isSaving || $scope.isDeleting || !$scope.userarray[userindex]) {
                return;
            }
            var user = $scope.userarray[userindex];
            $scope.isDeleting = true;
            Swal.fire({
                title: "Delete Member?",
                text: "Remove " + user.FName + " " + user.LName + " and their account from the database? This cannot be undone.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#c62828",
                cancelButtonColor: "#616161",
                confirmButtonText: "Yes, delete member"
            }).then(function (result) {
                $scope.$applyAsync(function () {
                    if (!result.isConfirmed) {
                        $scope.isDeleting = false;
                        return;
                    }
                    OrgConnectService.RemoveMemberFunc(user.DatabaseID).then(function (returnedData) {
                        $scope.isDeleting = false;
                        if (!returnedData.data.Success) {
                            Swal.fire({ title: "Delete Failed", text: returnedData.data.Message, icon: "error", confirmButtonColor: "#0a4caf" });
                            return;
                        }
                        $scope.userarray = returnedData.data.Members;
                        $scope.clearRegistrationFunc();
                        Swal.fire({ title: "Delete Successful", text: "The member and account were deleted from the database.", icon: "success", confirmButtonColor: "#0a4caf" });
                    }, function () {
                        $scope.isDeleting = false;
                        Swal.fire({ title: "Delete Failed", text: "The request failed. Check the database connection before trying again.", icon: "error", confirmButtonColor: "#0a4caf" });
                    });
                });
            });
        };

        // clear registration form

        $scope.clearRegistrationFunc = function (registrationForm) {
            $scope.isEditing = false;
            $scope.editingIndex = -1;
            $scope.serverErrors = {};
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

        $scope.databaseStatus = "Not checked";
        $scope.databaseResult = null;
        $scope.checkDatabaseFunc = function () {
            $scope.databaseStatus = "Checking connection...";
            $scope.databaseResult = null;
            OrgConnectService.CheckDatabaseFunc().then(function (returnedData) {
                $scope.databaseResult = returnedData.data;
                $scope.databaseStatus = returnedData.data.Success
                    ? "Database connected successfully."
                    : returnedData.data.Message;
            }, function () {
                $scope.databaseStatus = "Database check failed. Check the connection settings and database setup.";
            });
        };

    });


