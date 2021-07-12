using ev;
using events;

namespace events.handlers {
    public class FigurePickEvent {

        public Demux<FigPickEvent> handler = new Demux<FigPickEvent>();
    }
}