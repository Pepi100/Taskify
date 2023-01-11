window.addEventListener("load", function () {
    var mytasks = document.getElementById("flexSwitchCheckDefault");
    ///de sters console log
    mytasks.onchange = function () {
        let flag = false;
        if (mytasks.checked) {
            flag = true;
        }


        let user_sesiune = document.getElementById("id_user_sesiune").innerHTML.trim();
        ///console.log(user_sesiune);

        var tasks = document.getElementsByClassName("taskuri");
        var task_front = document.getElementsByClassName("taskuri-asoc");
        var i = 0;
        for (let task of tasks) {
            task.style.display = "none";
            task_front[i].style.display = "none";
            let user_task = task.getElementsByClassName("id_user_task")[0].innerHTML.trim();
            ////console.log(user_task);
            let conditie_finala = (user_sesiune == user_task);
            if (conditie_finala || !flag) {
                task.style.display = "block";
                task_front[i].style.display = "block";
            }
            i++;
        }
    }
});
