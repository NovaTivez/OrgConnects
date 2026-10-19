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

            Swal.fire({ title: "Login Successful", text: "Welcome to OrgConnect!", icon: "success", confirmButtonColor: "#0a4caf" }).then(function () {
                window.location.href = "/Main/Index";
            });
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
        $scope.committeeList = [
            "Executive Committee", "Membership Committee", "Events Committee",
            "Finance Committee", "Marketing Committee", "Creatives Committee",
            "Documentation Committee"
        ];
        $scope.positionList = [];

        $scope.registrationSubmitted = false;
        $scope.isEditing = false;
        $scope.editingIndex = -1;


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
                return index !== $scope.editingIndex &&
                    user.MemberID.toLowerCase() === enteredID;
            });
        };

        $scope.isDuplicateUsername = function () {
            var enteredUsername = ($scope.username || "").toLowerCase();

            return $scope.userarray.some(function (user, index) {
                return index !== $scope.editingIndex &&
                    (user.Username || "").toLowerCase() === enteredUsername;
            });
        };

        $scope.isDuplicateEmail = function () {
            var enteredEmail = ($scope.email || "").trim().toLowerCase();

            return $scope.userarray.some(function (user, index) {
                return index !== $scope.editingIndex &&
                    (user.Email || "").trim().toLowerCase() === enteredEmail;
            });
        };

        $scope.isDuplicateContactNumber = function () {
            var enteredNumber = $scope.contactNumber || "";

            return $scope.userarray.some(function (user, index) {
                return index !== $scope.editingIndex &&
                    user.ContactNumber === enteredNumber;
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
            if ($scope.committee === "Executive Committee") {
                $scope.positionList = ["President", "Vice President", "Secretary", "Treasurer"];
            } else if ($scope.committee) {
                $scope.positionList = ["Committee Head", "Officer", "Member"];
            } else {
                $scope.positionList = [];
            }
        };


        // create

        $scope.registrationFunc = function (registrationForm) {
            $scope.registrationSubmitted = true;

            if (registrationForm.$invalid) {
                Swal.fire({ title: "Invalid Information", text: "Please correct the highlighted fields.", icon: "error", confirmButtonColor: "#0a4caf" });
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



            $scope.userarray.push({
                MemberID: $scope.memberID.toUpperCase(),
                FName: $scope.firstName,
                LName: $scope.lastName,
                Username: $scope.username,
                Email: $scope.email.trim().toLowerCase(),
                ContactNumber: $scope.contactNumber,
                Password: $scope.password,
                Committee: $scope.committee,
                Position: $scope.position
            });

            Swal.fire({ title: "Registration Successful", text: "The organization member was registered.", icon: "success", confirmButtonColor: "#0a4caf" });

            $scope.clearRegistrationFunc(registrationForm);
        };


        // reaad and edit

        $scope.editFunc = function (userindex) {
            var user = $scope.userarray[userindex];

            $scope.editingIndex = userindex;
            $scope.isEditing = true;

            $scope.memberID = user.MemberID;
            $scope.firstName = user.FName;
            $scope.lastName = user.LName;
            $scope.username = user.Username;
            $scope.email = user.Email;
            $scope.contactNumber = user.ContactNumber;
            $scope.password = user.Password;
            $scope.confirmPassword = user.Password;
            $scope.committee = user.Committee;
            if ($scope.committee === "Executive Committee") {
                $scope.positionList = ["President", "Vice President", "Secretary", "Treasurer"];
            } else if ($scope.committee) {
                $scope.positionList = ["Committee Head", "Officer", "Member"];
            } else {
                $scope.positionList = [];
            }
            $scope.position = user.Position;

            setTimeout(function () {
                if (window.M && window.M.updateTextFields) {
                    window.M.updateTextFields();
                }
            }, 0);

            window.scrollTo({
                top: 0,
                behavior: "smooth"
            });
        };


        // update

        $scope.updateFunc = function (registrationForm) {
            $scope.registrationSubmitted = true;

            if (registrationForm.$invalid) {
                Swal.fire({ title: "Invalid Information", text: "Please correct the highlighted fields.", icon: "error", confirmButtonColor: "#0a4caf" });
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



            $scope.userarray[$scope.editingIndex] = {
                MemberID: $scope.memberID.toUpperCase(),
                FName: $scope.firstName,
                LName: $scope.lastName,
                Username: $scope.username,
                Email: $scope.email.trim().toLowerCase(),
                ContactNumber: $scope.contactNumber,
                Password: $scope.password,
                Committee: $scope.committee,
                Position: $scope.position
            };

            Swal.fire({ title: "Update Successful", text: "The member information was updated.", icon: "success", confirmButtonColor: "#0a4caf" });

            $scope.clearRegistrationFunc(registrationForm);
        };


        // delete

        $scope.deleteFunc = function (userindex) {
            var user = $scope.userarray[userindex];

            Swal.fire({
                title: "Delete Member?",
                text: "Remove " + user.FName + " " + user.LName + "?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#c62828",
                cancelButtonColor: "#616161",
                confirmButtonText: "Yes, delete"
            }).then(function (result) {
                if (!result.isConfirmed) {
                    return;
                }

                $scope.$apply(function () {
                    $scope.userarray.splice(userindex, 1);

                    if ($scope.editingIndex === userindex) {
                        $scope.clearRegistrationFunc();
                    } else if (userindex < $scope.editingIndex) {
                        $scope.editingIndex--;
                    }
                });

                Swal.fire({ title: "Deleted", text: "The member record was removed.", icon: "success", confirmButtonColor: "#0a4caf" });
            });
        };


        // clear and cancel

        $scope.clearRegistrationFunc = function (registrationForm) {
            $scope.memberID = "";
            $scope.firstName = "";
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
            $scope.isEditing = false;
            $scope.editingIndex = -1;

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

        $scope.cancelEditFunc = function (registrationForm) {
            $scope.clearRegistrationFunc(registrationForm);
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


