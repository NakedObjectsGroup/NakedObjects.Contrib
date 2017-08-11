using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cluster.Countries.Api;

namespace Cluster.Addresses.Api
{
    /// <summary>
    /// Role interface intended to be implemented by any object that can play the role of a postal address e.g.provide the lines for 
    /// an address label.
    /// </summary>
    public interface IPostalAddress
    {

        /// <summary>
        /// Representation of complete address in a single line with commas; will often be
        /// held (i.e. denormalised) by other objects for convenience of viewing. 
        /// </summary>
        string AsSingleLine();

        /// <summary>
        /// Representation of full address as an array of lines (no blank lines) suitable for use
        /// in a mailing label
        /// </summary>
        /// <returns></returns>
        string[] LabelFormIncludingCountry();


        /// <summary>
        /// If the address is in the country specified, then the Country is not appended.
        /// </summary>
        string[] LabelFormAsPostedFrom(ICountry country);

        /// <summary>
        /// The country in which this postal address is located (does not imply anything about
        /// the location of the person or othe object that this address might 'belong' to).
        /// </summary>
        ICountry DestinationCountry();
    }
}
