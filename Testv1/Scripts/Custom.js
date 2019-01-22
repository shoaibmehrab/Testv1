<script>

    

    var app = angular.module('myApp', []);
        app.controller('validateCtrl', function ($scope) {

    });


        function uploadCheck() {
            var syllabusfile = document.getElementById('syllabusfile');
    var testfile = document.getElementById('testPlanFile');
    var file1Name = syllabusfile.value;
    var file2Name = testfile.value;
    var syllabusExt = file1Name.substring(file1Name.lastIndexOf('.') + 1);
    var testplanExt = file2Name.substring(file2Name.lastIndexOf('.') + 1);
            if ((syllabusExt == "pdf" || syllabusExt == "PDF") && (testplanExt == "pdf" || testplanExt == "PDF")) {
                return true;
               }
            else {
        syllabusFileAlert.innerText = "Syllabus should be pdf format";
    testPlanFileALert.innerText = "Test Plan should be pdf format";
    syllabusfile.focus();
    testfile.focus();
    return false;
}

}

</script>

<