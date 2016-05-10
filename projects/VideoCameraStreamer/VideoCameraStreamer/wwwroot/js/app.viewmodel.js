// Class to represent a row in the seat reservations grid
function VideoInfo(name, src, poster) {
    var self = this;
    self.name = name;
    self.src = src;
    self.poster = poster;
}

function ViewModel(videosList) {
    var self = this;
    
    //self.videos = ko.observableArray(videosList);

    self.videos = ko.observableArray([
        new VideoInfo("Steve", "http://techslides.com/demos/sample-videos/small.mp4", "https://upload.wikimedia.org/wikipedia/commons/e/e8/Chief_Mountain.jpg"),
      //  new VideoInfo("Bert", "http://test.themefuse.com/artiom/galapagos.mp4", "images/temp/vjs_poster2.png"),
     //   new VideoInfo("Bert", "http://test.themefuse.com/artiom/galapagos.mp4", "images/temp/vjs_poster2.png"),
      //  new VideoInfo("Bert", "http://test.themefuse.com/artiom/galapagos.mp4", "images/temp/vjs_poster2.png")
    ]);

    self.videoRemoved = function (video) {
        self.videos.pop(video);
    }

    self.videoAdded = function (video) {
        self.videos.push(video);
    }
};

ko.applyBindings(new ViewModel());