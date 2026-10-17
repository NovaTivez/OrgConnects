app.controller("OrgConnectController",
    function ($scope, OrgConnectService) {

        function showMessage(title, text, icon) {
            return Swal.fire({
                title: title,
                text: text,
                icon: icon,
                confirmButtonColor: "#0a4caf"
            });
        }

        function updateLabels() {
            setTimeout(function () {
                if (window.M && window.M.updateTextFields) {
                    window.M.updateTextFields();
                }
            }, 0);
        }

        function getMemberData() {
            return {
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
        }

        function formIsInvalid(registrationForm) {
            if (registrationForm.$invalid) {
                showMessage(
                    "Invalid Information",
                    "Please correct the highlighted fields.",
                    "error"
                );
                return true;
            }

            return false;
        }

        // checks every field that must be unique
        function hasDuplicateInformation() {
            if ($scope.isDuplicateMemberID()) {
                showMessage(
                    "Duplicate Member ID",
                    "The entered Member ID already exists.",
                    "error"
                );
                return true;
            }

            if ($scope.isDuplicateUsername()) {
                showMessage(
                    "Duplicate Username",
                    "The entered username already exists.",
                    "error"
                );
                return true;
            }

            if ($scope.isDuplicateEmail()) {
                showMessage(
                    "Duplicate Email",
                    "The entered email address already exists.",
                    "error"
                );
                return true;
            }

            if ($scope.isDuplicateContactNumber()) {
                showMessage(
                    "Duplicate Contact Number",
                    "The entered contact number already exists.",
                    "error"
                );
                return true;
            }

            return false;
        }


        // login

        $scope.loginSubmitted = false;

        $scope.loginFunc = function (loginForm) {
            $scope.loginSubmitted = true;

            if (loginForm.$invalid) {
                showMessage(
                    "Login Failed",
                    "Please enter a valid username and password.",
                    "error"
                );
                return;
            }

            showMessage(
                "Login Successful",
                "Welcome to OrgConnect!",
                "success"
            ).then(function () {
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
        $scope.committeeList = OrgConnectService.getCommittees();
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
            $scope.positionList =
                OrgConnectService.getPositions($scope.committee);
        };


        // create

        $scope.registrationFunc = function (registrationForm) {
            $scope.registrationSubmitted = true;

            if (formIsInvalid(registrationForm) || hasDuplicateInformation()) {
                return;
            }

            $scope.userarray.push(getMemberData());

            showMessage(
                "Registration Successful",
                "The organization member was registered.",
                "success"
            );

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
            $scope.positionList =
                OrgConnectService.getPositions($scope.committee);
            $scope.position = user.Position;

            updateLabels();

            window.scrollTo({
                top: 0,
                behavior: "smooth"
            });
        };


        // update

        $scope.updateFunc = function (registrationForm) {
            $scope.registrationSubmitted = true;

            if (formIsInvalid(registrationForm) || hasDuplicateInformation()) {
                return;
            }

            $scope.userarray[$scope.editingIndex] = getMemberData();

            showMessage(
                "Update Successful",
                "The member information was updated.",
                "success"
            );

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

                showMessage(
                    "Deleted",
                    "The member record was removed.",
                    "success"
                );
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

            updateLabels();
        };

        $scope.cancelEditFunc = function (registrationForm) {
            $scope.clearRegistrationFunc(registrationForm);
        };

    });


