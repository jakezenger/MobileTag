using MobileTag.Models;

namespace MobileTag.SharedCode
{
    public interface IMapOverlayClickHandler
    {
        void HandleClickEvent(MapActivity mapActivity, Cell cell);
    }
}