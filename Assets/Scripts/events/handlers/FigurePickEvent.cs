using ev;
using events;

namespace Assets.Scripts.events.handlers {
    public class FigurePickEvent {

        public Demux<FigPickEvent> handler = new Demux<FigPickEvent>();
    }
}