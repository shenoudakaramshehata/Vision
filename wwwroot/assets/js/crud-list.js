

checkAll = document.getElementById("checkAll"),
    perPage =
    (checkAll &&
        (checkAll.onclick = function () {
            for (
                var e = document.querySelectorAll(
                    '.form-check-all input[type="checkbox"]'
                ),
                t = 0;
                t < e.length;
                t++
            )
                (e[t].checked = this.checked),
                    e[t].checked
                        ? e[t].closest("tr").classList.add("table-active")
                        : e[t].closest("tr").classList.remove("table-active");
        }),
        8);



options = {
    valueNames: [
        "CategoryId",
        "CategoryEn",
        "CategoryTitleAr",
        { attr: "src", name: "CategoryPic" },
    ],
    page: perPage,
    pagination: !0,
    plugins: [ListPagination({ left: 2, right: 2 })],
}

leadsList = new List("List", options).on("updated", function (e) {
    0 == e.matchingItems.length
        ? (document.getElementsByClassName("noresult")[0].style.display = "block")
        : (document.getElementsByClassName("noresult")[0].style.display = "none");
    var t = 1 == e.i,
        a = e.i > e.matchingItems.length - e.page;
    document.querySelector(".pagination-prev.disabled") &&
        document
            .querySelector(".pagination-prev.disabled")
            .classList.remove("disabled"),
        document.querySelector(".pagination-next.disabled") &&
        document
            .querySelector(".pagination-next.disabled")
            .classList.remove("disabled"),
        t && document.querySelector(".pagination-prev").classList.add("disabled"),
        a && document.querySelector(".pagination-next").classList.add("disabled"),
        e.matchingItems.length <= perPage
            ? (document.querySelector(".pagination-wrap").style.display = "none")
            : (document.querySelector(".pagination-wrap").style.display = "flex"),
        0 < e.matchingItems.length
            ? (document.getElementsByClassName("noresult")[0].style.display =
                "none")
            : (document.getElementsByClassName("noresult")[0].style.display =
                "block");
});


document
    .querySelector(".pagination-next")
    .addEventListener("click", function () {
        !document.querySelector(".pagination.listjs-pagination") ||
            (document
                .querySelector(".pagination.listjs-pagination")
                .querySelector(".active") &&
                document
                    .querySelector(".pagination.listjs-pagination")
                    .querySelector(".active")
                    .nextElementSibling.children[0].click());
    });


document
    .querySelector(".pagination-prev")
    .addEventListener("click", function () {
        !document.querySelector(".pagination.listjs-pagination") ||
            (document
                .querySelector(".pagination.listjs-pagination")
                .querySelector(".active") &&
                document
                    .querySelector(".pagination.listjs-pagination")
                    .querySelector(".active")
                    .previousSibling.children[0].click());
    });
