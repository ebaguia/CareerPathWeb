﻿// Copyright (c) 2016 University of Auckland. All rights reserved.
//
//

var PAPER;
var NUMBER_OF_YEARS = 4;
var COLUMN_WIDTH;
var BUTTON_STARTX1 = 20;
var BUTTON_STARTX2 = 60;
var ROW_TOTAL = 12;

/**
 * Class the defines the Course object
 **/
var Course = function (year,
                        sem,
                        id,
                        name,
                        description,
                        academicOrg,
                        academicGroup,
                        courseComp,
                        gradingBasis,
                        typeOffered,
                        preReqString,
                        restrString,
                        points,
                        xcoor,
                        ycoor,
                        top,
                        compulsory) {
    this.year = year;
    this.sem = sem;
    this.id = id;
    this.name = name;
    this.description = description;
    this.academicOrg = academicOrg;
    this.academicGroup = academicGroup;
    this.courseComp = courseComp;
    this.gradingBasis = gradingBasis;
    this.typeOffered = typeOffered;
    this.preReqString = preReqString;
    this.restrString = restrString;
    this.points = points;
    this.xcoor = xcoor;
    this.ycoor = ycoor;
    this.top = top;
    this.compulsory = compulsory;
};

/**
 * Function to setup all the information
 **/
function initLayout() {
    PAPER = new Raphael(document.getElementById("coursepathpanel"), "100%", "100%");
    PAPER.clear();

    var papersize = PAPER.getSize();
    var x = 0;
    var y = 0;
    var height = 50;
    COLUMN_WIDTH = papersize.width / NUMBER_OF_YEARS;

    // Creating column headers
    //
    for (var i = 0; i < NUMBER_OF_YEARS; i++) {
        writeTableRow(x, y, COLUMN_WIDTH, height, PAPER, "YEAR " + (i + 1));
        x += COLUMN_WIDTH;
    }

    // Creating columns
    //
    var tempWidth = COLUMN_WIDTH;
    for (var i = 0; i < NUMBER_OF_YEARS; i++) {
        if (i > 0 && i < NUMBER_OF_YEARS) {
            PAPER.line(tempWidth, height, tempWidth, papersize.height).attr({
                "fill": "white", "stroke": "black", "stroke-dasharray": "--"
            });
            tempWidth += COLUMN_WIDTH;
        }
    }
}

/**
 * To be called first
 **/
function drawSomething(arg) {
    initLayout();
    
    var courseButtons = [];

    if (arg.Courses.length != 0) {
        for (var i = 0; i < arg.Courses.length; i++) {
            var json = arg.Courses[i];

            // Parse the information being passed
            //
            var json_parsed = JSON.parse(JSON.stringify(json));
            var courseinfo = new Course(json_parsed.year,
                json_parsed.sem,
                json_parsed.id,
                json_parsed.name,
                json_parsed.description,
                json_parsed.academicOrg,
                json_parsed.academicGroup,
                json_parsed.courseComp,
                json_parsed.gradingBasis,
                json_parsed.typeOffered,
                json_parsed.preReqString,
                json_parsed.restrString,
                json_parsed.points,
                json_parsed.xcoor,
                json_parsed.ycoor,
                json_parsed.top,
                json_parsed.compulsory);

            var is_course_exist = false;
            var j = 0;
            for (; j < courseButtons.length; j++) {
                if (courseButtons[j].courseinfo.id == courseinfo.id) {
                    is_course_exist = true;
                    break;
                }
            }

            // Make sure we do not have replicate courses to be displayed
            //
            if (!is_course_exist) {
                courseButtons.push({
                    courseinfo: courseinfo,
                    button: createCourseButton(COLUMN_WIDTH - 80, 50, PAPER, courseinfo),
                    tops: [courseinfo.top]
                });
            }
            else {
                // Each course will have one or more pre-requisites
                //
                courseButtons[j].tops.push(courseinfo.top);
            }
        }

        // Write down each course ID in the button
        //
        for (var i = 0; i < courseButtons.length; i++) {
            // Draw the lines connecting all related courses
            //
            var button = courseButtons[i].button;
            for (var j = 0; j < courseButtons[i].tops.length; j++) {
                for (var k = 0; k < courseButtons.length; k++) {
                    // Get all related courses of this pre-requisite course and connect to them
                    //
                    if (courseButtons[k].courseinfo.id == courseButtons[i].tops[j]) {
                        var courseend = courseButtons[k].button;
                        PAPER.connection(button, courseend, "purple");
                    }
                }
            }
        }
    }
}

function focusOnCourse(courseinfo) {
    var rows = [];
    var legend;

    legend = document.getElementById("legend");

    // Clean the table
    //
    legend.innerHTML = "";

    // Define the column headers
    //
    addCourseInfoHeader(legend);

    // There are ROW_TOTAL rows to be made
    //
    for (i = 1; i <= ROW_TOTAL; i++) {
        var row = legend.insertRow(i);
        rows.push(row);
    }
    
    addCourseInfoRow(rows[1], "ID", courseinfo.id);
    addCourseInfoRow(rows[2], "Name", courseinfo.name);
    addCourseInfoRow(rows[3], "Type", courseinfo.compulsory == 1 ? "Compulsory" : "Elective");
    addCourseInfoRow(rows[4], "Description", courseinfo.description);
    addCourseInfoRow(rows[5], "Academic Organization", courseinfo.academicOrg);
    addCourseInfoRow(rows[6], "Academic Group", courseinfo.academicGroup);
    addCourseInfoRow(rows[7], "Course Component", courseinfo.courseComp);
    addCourseInfoRow(rows[8], "Grading Basis", courseinfo.gradingBasis);
    addCourseInfoRow(rows[9], "Typically Offered", courseinfo.typeOffered);
    addCourseInfoRow(rows[10], "Prerequisite(s)", courseinfo.preReqString);
    addCourseInfoRow(rows[11], "Restrictions(s)", courseinfo.restrString);

    var modal = document.getElementById('courseInfoModal');
    modal.style.display = "block";

    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];

    // When the user clicks on <span> (x), close the modal
    span.onclick = function () {
        modal.style.display = "none";
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    }

}

//
// The first row of the course information table is the header
//
function addCourseInfoHeader(legend) {
    var header = legend.createTHead();
    var row = header.insertRow(0);
    var cell1 = row.insertCell(0);
    cell1.innerHTML = "<h4>COURSE</h4>";
    cell1.style.backgroundColor = "#f1f1f1";
    $(cell1).css("text-align", "center");

    var cell2 = row.insertCell(1);
    cell2.innerHTML = "<h4>INFORMATION</h4>";
    cell2.style.backgroundColor = "#f1f1f1";
    $(cell2).css("text-align", "center");
}

function addCourseInfoRow(table_row, item, desc) {
    var cell1 = table_row.insertCell(0);
    cell1.innerHTML = "<b>" + item + "</b>";

    var cell2 = table_row.insertCell(1);
    cell2.innerHTML = desc;
}

//
// Helper functions
//
Raphael.fn.line = function(startX, startY, endX, endY){
    return this.path('M' + startX + ' ' + startY + ' L' + endX + ' ' + endY);
};

function writeTableRow(x, y, width, height, paper, TDdata) {
    var TD = TDdata.split(",");
    for (j = 0; j < TD.length; j++) {
        var rect = paper.rect(x, y, width, height).attr({
            "fill": "#f1f1f1", "stroke": "black"
        });
        paper.text(x + width / 2, y + height / 2, TD[j]).attr({
            "font-family": "Arial",
            "font-size": 12,
            "font-weight": "bold"
        });
        x = x + width;
    }
}

function createCourseButton(width, height, paper, courseinfo) {
    var years = [];
    var bbox;
    
    for (var i = 0; i < NUMBER_OF_YEARS; i++) {
        years[i] = 0;
        if(i > 0) {
            years[i] = COLUMN_WIDTH * i;
        }
    }

    var bboxstartx = BUTTON_STARTX1;
    if (courseinfo.sem == 2) {
        bboxstartx = BUTTON_STARTX2;
    }

    if (courseinfo.year == 1) {
        bbox = paper.rect(years[0] + bboxstartx, courseinfo.ycoor, width, height);
    }
    else if (courseinfo.year == 2) {
        bbox = paper.rect(years[1] + bboxstartx, courseinfo.ycoor, width, height);
    }
    else if (courseinfo.year == 3) {
        bbox = paper.rect(years[2] + bboxstartx, courseinfo.ycoor, width, height);
    }
    else if (courseinfo.year == 4) {
        bbox = paper.rect(years[3] + bboxstartx, courseinfo.ycoor, width, height);
    }
    if(courseinfo.compulsory == 1) {
        bbox.attr({
            "fill": "orangered",
            "stroke": "orangered",
            "cursor": "pointer"
        });
    }
    else {
        bbox.attr({
            "fill": "lightgreen",
            "stroke": "lightgreen",
            "cursor": "pointer"
        });
    }

    // Text inside the box
    //
    var buttontext = paper.text(bbox.attrs.x + bbox.attrs.width / 2,
        bbox.attrs.y + bbox.attrs.height / 2,
        courseinfo.id).attr({
            "font-family": "Arial",
            "font-size": 12,
            "cursor": "pointer"
        });

    // Create mouse events
    //
    var button = paper.set().attr({
        cursor: 'pointer'
    });
    button.push(bbox);
    button.push(buttontext);

    button.mouseover(function (event) {
        this.oGlow = bbox.glow({
            opacity: 0.85,
            color: 'gray',
            width: 10
        });
    }).mouseout(function (event) {
        this.oGlow.remove();
    }).mouseup(function (e) {
        focusOnCourse(courseinfo);
    });

    return bbox;
}

//
// Reference: http://raphaeljs.com/graffle.js
//
Raphael.fn.connection = function (obj1, obj2, line, bg) {
    if (obj1.line && obj1.from && obj1.to) {
        line = obj1;
        obj1 = line.from;
        obj2 = line.to;
    }
    var bb1 = obj1.getBBox(),
        bb2 = obj2.getBBox(),
        p = [{ x: bb1.x + bb1.width / 2, y: bb1.y - 1 },
        { x: bb1.x + bb1.width / 2, y: bb1.y + bb1.height + 1 },
        { x: bb1.x - 1, y: bb1.y + bb1.height / 2 },
        { x: bb1.x + bb1.width + 1, y: bb1.y + bb1.height / 2 },
        { x: bb2.x + bb2.width / 2, y: bb2.y - 1 },
        { x: bb2.x + bb2.width / 2, y: bb2.y + bb2.height + 1 },
        { x: bb2.x - 1, y: bb2.y + bb2.height / 2 },
        { x: bb2.x + bb2.width + 1, y: bb2.y + bb2.height / 2 }],
        d = {}, dis = [];
    for (var i = 0; i < 4; i++) {
        for (var j = 4; j < 8; j++) {
            var dx = Math.abs(p[i].x - p[j].x),
                dy = Math.abs(p[i].y - p[j].y);
            if ((i == j - 4) || (((i != 3 && j != 6) || p[i].x < p[j].x) && ((i != 2 && j != 7) || p[i].x > p[j].x) && ((i != 0 && j != 5) || p[i].y > p[j].y) && ((i != 1 && j != 4) || p[i].y < p[j].y))) {
                dis.push(dx + dy);
                d[dis[dis.length - 1]] = [i, j];
            }
        }
    }
    if (dis.length == 0) {
        var res = [0, 4];
    } else {
        res = d[Math.min.apply(Math, dis)];
    }
    var x1 = p[res[0]].x,
        y1 = p[res[0]].y,
        x4 = p[res[1]].x,
        y4 = p[res[1]].y;
    dx = Math.max(Math.abs(x1 - x4) / 2, 10);
    dy = Math.max(Math.abs(y1 - y4) / 2, 10);
    var x2 = [x1, x1, x1 - dx, x1 + dx][res[0]].toFixed(3),
        y2 = [y1 - dy, y1 + dy, y1, y1][res[0]].toFixed(3),
        x3 = [0, 0, 0, 0, x4, x4, x4 - dx, x4 + dx][res[1]].toFixed(3),
        y3 = [0, 0, 0, 0, y1 + dy, y1 - dy, y4, y4][res[1]].toFixed(3);
    var path = ["M", x1.toFixed(3), y1.toFixed(3), "C", x2, y2, x3, y3, x4.toFixed(3), y4.toFixed(3)].join(",");
    if (line && line.line) {
        line.bg && line.bg.attr({ path: path });
        line.line.attr({ path: path });
    } else {
        var color = typeof line == "string" ? line : "#000";
        return {
            bg: bg && bg.split && this.path(path).attr({ stroke: bg.split("|")[0], fill: "none", "stroke-width": bg.split("|")[1] || 3 }),
            line: this.path(path).attr({ stroke: color, fill: "none", "arrow-end": "block-wide-long" }),
            from: obj1,
            to: obj2
        };
    }
};