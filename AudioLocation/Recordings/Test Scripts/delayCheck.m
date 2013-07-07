speakerSpikes = [
21729      198121      374481      550873;
65633      242057      418449      594873;
109281      285705      462161     638553;
153505      329865      506289      682777
];
fs = 44100;
speakerSpikes = speakerSpikes(:)/fs
speakerSpikes = speakerSpikes-(0:length(speakerSpikes)-1)';
diff(speakerSpikes)'

