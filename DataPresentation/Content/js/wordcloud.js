/**
 * Created by darren on 2/9/17.
 */

function wordCloud(selector) {
    var fill = d3.scale.category20();
    
    //Construct the word cloud's SVG element
    var svg = d3.select(selector).append("svg")
        .attr("width", 800)
        .attr("height", 500)
        .append("g")
        .attr("transform", "translate(350,250)");

    //Draw the word cloud
    function draw(words) {
        var cloud = svg.selectAll("g text")
            .data(words, function(d) { return d.text; });
        
        //Entering words
        cloud.enter()
            .append("text")
            .style("font-family", "Impact")
            .style("fill", function(d, i) { return fill(i); })
            .attr("text-anchor", "middle")
            .attr('font-size', 1)
            .text(function(d) { return d.text; });

        //Entering and existing words
        cloud.transition()
            .duration(600)
            .style("font-size", function(d) { return d.size + "px"; })
            .attr("transform", function(d) {
                return "translate(" + [d.x, d.y] + ")rotate(" + d.rotate + ")";
            })
            .style("fill-opacity", 1);

        //Exiting words
        cloud.exit()
            .transition()
            .duration(200)
            .style('fill-opacity', 1e-6)
            .attr('font-size', 1)
            .remove();
    }

    //Use the module pattern to encapsulate the visualisation code. We'll
    // expose only the parts that need to be public.
    
    return {
        //Recompute the word cloud for a new set of words. This method will
        // asycnhronously call draw when the layout has been computed.
        //The outside world will need to call this function, so make it part
        // of the wordCloud return value.
        update: function(words) {
            d3.layout.cloud().size([800, 500])
                .words(words)
                .padding(8)
                .rotate(function() { return ~~(Math.random() * 2) * 90; })
                .font("Impact")
                .fontSize(function(d) { return Math.sqrt(d.size); })
                .on("end", draw)
                .start();
        }
    }
}
//Some sample data - http://en.wikiquote.org/wiki/Opening_lines
var words = [
    "You don't know about me without you have read a book called The Adventures of Tom Sawyer but that ain't no matter.",
    "The boy with fair hair lowered himself down the last few feet of rock and began to pick his way toward the lagoon.",
    "When Mr. Bilbo Baggins of Bag End announced that he would shortly be celebrating his eleventy-first birthday with a party of special magnificence, there was much talk and excitement in Hobbiton.",
    "It was inevitable: the scent of bitter almonds always reminded him of the fate of unrequited love."
];

var list = [{"size":20832,"text":"car"},{"size":10340,"text":"front"},{"size":9093,"text":"airbags"},{"size":8905,"text":"power"},{"size":7872,"text":"control"},{"size":7338,"text":"rear"},{"size":6502,"text":"steering"},{"size":5409,"text":"features"},{"size":4891,"text":"driver"},{"size":4862,"text":"wheel"},{"size":4410,"text":"central"},{"size":4395,"text":"leather"},{"size":4273,"text":"locking"},{"size":4193,"text":"air"},{"size":4178,"text":"seats"},{"size":4136,"text":"passenger"},{"size":4063,"text":"wheels"},{"size":4048,"text":"great"},{"size":3947,"text":"windows"},{"size":3907,"text":"alloy"},{"size":3893,"text":"safety"},{"size":3680,"text":"cruise"},{"size":3661,"text":"roadworthy"},{"size":3625,"text":"remote"},{"size":3594,"text":"side"},{"size":3489,"text":"brakes"},{"size":3409,"text":"conditioning"},{"size":3339,"text":"bluetooth"},{"size":3262,"text":"holders"},{"size":3188,"text":"km"},{"size":3148,"text":"brake"},{"size":3124,"text":"abs"},{"size":3063,"text":"cup"},{"size":3043,"text":"certificate"},{"size":3034,"text":"family"},{"size":2830,"text":"door"},{"size":2741,"text":"condition"},{"size":2668,"text":"fitted"},{"size":2656,"text":"system"},{"size":2586,"text":"service"},{"size":2569,"text":"audio"},{"size":2518,"text":"full"},{"size":2509,"text":"vehicle"},{"size":2357,"text":"engine"},{"size":2289,"text":"mirrors"},{"size":2225,"text":"assist"},{"size":2223,"text":"electronic"},{"size":2154,"text":"love"},{"size":2057,"text":"multi"},{"size":1972,"text":"rating"},{"size":1950,"text":"ancap"},{"size":1947,"text":"protection"},{"size":1942,"text":"climate"},{"size":1915,"text":"connectivity"},{"size":1899,"text":"parking"},{"size":1845,"text":"lamps"},{"size":1812,"text":"owner"},{"size":1798,"text":"sell"},{"size":1751,"text":"function"},{"size":1729,"text":"usb"},{"size":1720,"text":"price"},{"size":1665,"text":"serviced"},{"size":1659,"text":"drive"},{"size":1649,"text":"toyota"},{"size":1609,"text":"airbag"},{"size":1602,"text":"input"},{"size":1584,"text":"electric"},{"size":1568,"text":"litre"},{"size":1550,"text":"clock"},{"size":1538,"text":"exceptional"},{"size":1511,"text":"tyres"},{"size":1478,"text":"sensors"},{"size":1477,"text":"history"},{"size":1434,"text":"registration"},{"size":1425,"text":"travelled"},{"size":1424,"text":"phone"},{"size":1422,"text":"don"},{"size":1346,"text":"priced"},{"size":1336,"text":"driving"},{"size":1310,"text":"represents"},{"size":1305,"text":"sale"},{"size":1293,"text":"fog"},{"size":1291,"text":"selling"},{"size":1289,"text":"perfect"},{"size":1265,"text":"storage"},{"size":1265,"text":"powerful"},{"size":1264,"text":"included"},{"size":1200,"text":"ipod"},{"size":1185,"text":"good"},{"size":1183,"text":"map"},{"size":1183,"text":"camera"},{"size":1165,"text":"holden"},{"size":1156,"text":"paid"},{"size":1146,"text":"row"},{"size":1146,"text":"zone"},{"size":1131,"text":"added"},{"size":1116,"text":"force"},{"size":1104,"text":"fuel"},{"size":1103,"text":"months"},{"size":1101,"text":"rego"}]
var templist = [];
//Prepare one of the sample sentences by removing punctuation,
// creating an array of words and computing a random size attribute.
function getWords(i) {
    return list
        .map(function(d) {
            return d;
        })
}
hasChanged = false;

function updateCanvas(vis) {
    vis.update(getWords())
}

function showNewWords(vis, wordslist) {
    //i = i || 0;
    list = wordslist;
    vis.update(getWords(wordslist.length))
    //setTimeout(function() { showNewWords(vis)}, 2000)

}

//Create a new instance of the word cloud visualisation.
var myWordCloud = wordCloud('div#wc');

//Start cycling through the demo data
showNewWords(myWordCloud, list);

function myFunction() {
    showNewWords(myWordCloud);
}
