using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRasta.Web;
using Paramore.Adapters.Infrastructure.Repositories;
using Paramore.Adapters.Presentation.API.Resources;
using Paramore.Domain.Common;
using Paramore.Domain.Venues;
using Paramore.Ports.Services.Commands.Venue;
using Paramore.Ports.Services.ThinReadLayer;
using paramore.commandprocessor;
using Version = Paramore.Adapters.Infrastructure.Repositories.Version;

namespace Paramore.Adapters.Presentation.API.Handlers
{
    public class VenueEndPointHandler
    {
        private readonly IAmAUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAmACommandProcessor commandProcessor;

        public VenueEndPointHandler(IAmAUnitOfWorkFactory unitOfWorkFactory, IAmACommandProcessor commandProcessor)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            this.commandProcessor = commandProcessor;
        }

        public OperationResult Get()
        {
            //var venues = new VenueReader(_unitOfWorkFactory, false).GetAll().ToList();
            var venues = Venues();

            return new OperationResult.OK
                    {
                        ResponseResource = venues
                    };
        }

        public OperationResult Post(VenueResource venueResource)
        {
            var venueCommand = new AddVenueCommand(
                venueName: venueResource.Name,
                address: venueResource.Address,
                mapURN: venueResource.MapURN,
                contact: venueResource.Contact);

            commandProcessor.Send(venueCommand);

            var venue = new VenueReader(_unitOfWorkFactory, false).Get(venueCommand.Id);
            
            return new OperationResult.OK
                    {
                        ResponseResource = venue
                    };
        }

        //DEBUG method to get results without hitting Db
        private List<VenueResource> Venues()
        {
            var venues = new List<VenueResource>
                {
                    new VenueResource(
                        id: new Id(Guid.NewGuid()),
                        version: new Version(1),
                        name: new VenueName("Test Venue"),
                        address: new Address(new Street(1, "MyStreet"), new City("London"), new PostCode("N1 3GA")),
                        mapURN: new VenueMap(new Uri("http://www.mysite.com/maps/12345")),
                        contact:
                            new Contact(new Name("Ian"), new EmailAddress("ian@huddle.com"),
                                             new PhoneNumber("123454678")))
                };
            return venues;
        }
    }
}