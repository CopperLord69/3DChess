using ev;

namespace events.handlers {
    public class FigureMoveEvent {

        public Demux<FigMoveEvent> handler = new Demux<FigMoveEvent>();
    }
}